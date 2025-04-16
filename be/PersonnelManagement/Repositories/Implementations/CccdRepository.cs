using PersonnelManagement.Helper;
using PersonnelManagement.Repositories.Interfaces;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using ZXing;
using ZXing.Common;
using ZXing.ZKWeb;
using System.DrawingCore;
using PersonnelManagement.Data;
using PersonnelManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using OpenCvSharp;
using PersonnelManagement.Models.DTOs;
using PersonnelManagement.Models.Entities;
using System.Globalization;

namespace PersonnelManagement.Repositories.Implementations
{
    public class CccdRepository : ICccdRepository
    {
        private readonly ILogger<CccdRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IFirebaseService _firebaseService;

        public CccdRepository(ILogger<CccdRepository> logger, ApplicationDbContext context, IFirebaseService firebaseService)
        {
            _logger = logger;
            _context = context;
            _firebaseService = firebaseService;
        }

        public async Task<Personnel?> ProcessCheckInOut(string numberId)
        {
            var personnel = await _context.Personnels
                .Include(p => p.RolePersonnels)
                .FirstOrDefaultAsync(p => p.NumberId == numberId);

            if (personnel == null)
            {
                return null;
            }

            // Toggle status
            personnel.Status = personnel.Status == Status.CheckIn ? Status.CheckOut : Status.CheckIn;
            personnel.UpdatedAt = DateTime.UtcNow.AddHours(7);

            // Create history record
            var history = new PersonnelHistory
            {
                Id = Guid.NewGuid(),
                PersonnelId = personnel.Id,
                Status = personnel.Status,
                Timestamp = DateTime.UtcNow.AddHours(7),
                Note = $"Automated {personnel.Status} via QR code",
                Personnel = personnel
            };

            _context.PersonnelHistories.Add(history);
            await _context.SaveChangesAsync();

            return personnel;
        }

        public async Task<List<PersonnelHistory>> GetPersonnelHistory()
        {
            return await _context.PersonnelHistories
                .Include(h => h.Personnel)
                .ThenInclude(p => p.RolePersonnels)
                .OrderByDescending(h => h.Timestamp)
                .Take(100) // Limit to last 100 records
                .ToListAsync();
        }

        public async Task<Personnel?> CheckAuthenByImage(IFormFile cccd)
        {
            // Get personnel info from CCCD image
            var personnel = await GetInfoCccdByImage(cccd, 2);

            if (personnel == null)
            {
                return null;
            }


            // Check if the user already exists 
            var existingUser = _context.Personnels
                .Include(rp => rp.RolePersonnels)
               .FirstOrDefault(u => u.NumberId == personnel.NumberId);

            if (existingUser != null)
            {
                existingUser.Status = existingUser.Status == Status.CheckIn ? Status.CheckOut : Status.CheckIn;
                existingUser.UpdatedAt = DateTime.UtcNow.AddHours(7);

                // Create history record
                var history = new PersonnelHistory
                {
                    Id = Guid.NewGuid(),
                    PersonnelId = existingUser.Id,
                    Status = existingUser.Status,
                    Timestamp = DateTime.UtcNow.AddHours(7),
                    Note = $"Automated {existingUser.Status} with image of CCCD",
                };

                _context.PersonnelHistories.Add(history);
                await _context.SaveChangesAsync();
                return existingUser;
            }

            return null;
        }

        private string RemoveDiacritics(string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            string result = string.Concat(normalizedString.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
            return result.ToLower(); // Chuyển thành chữ thường
        }

        public async Task<bool> AddPersonnel(IFormFile cccd, Guid rolePersonnelId, string phoneNumber)
        {
            try
            {
                // Get personnel info from CCCD image
                var personnel = await GetInfoCccdByImage(cccd, 1);

                if (personnel == null)
                {
                    return false;
                }

                // Check if the user already exists 
                var existingUser = _context.Personnels
                   .FirstOrDefault(u => u.NumberId == personnel.NumberId);

                if (existingUser != null)
                {
                    _logger.LogWarning($"User with CCCD {personnel.NumberId} already exists");
                    return false;
                }

                // Add the personnel to the context
                string lastName = RemoveDiacritics(personnel.FullName.Split(' ').Last());
                string lastFiveDigitsOfCccd = personnel.NumberId.Substring(personnel.NumberId.Length - 5);
                personnel.Id = lastName + lastFiveDigitsOfCccd;

                personnel.RolePersonnel = rolePersonnelId;
                personnel.PhoneNumber = phoneNumber;

                _context.Personnels.Add(personnel);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while adding personnel: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeletePersonnel(string id)
        {
            try
            {
                var personnel = await _context.Personnels.FindAsync(id);
                if (personnel == null)
                {
                    _logger.LogWarning($"Personnel with ID {id} not found");
                    return false;
                }

                await _firebaseService.DeleteFileAsync(personnel.AvatarUrl);
                _context.Personnels.Remove(personnel);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while deleting personnel: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdatePersonnel(string personnelId, UpdatePersonnel personnel)
        {
            try
            {
                var existingPersonnel = await _context.Personnels.FindAsync(personnelId);
                if (existingPersonnel == null)
                {
                    _logger.LogWarning($"Personnel with ID {personnelId} not found");
                    return false;
                }

                // Update properties
                if (personnel.rolePersonnelId != null)
                {
                    existingPersonnel.RolePersonnel = personnel.rolePersonnelId;
                }
                existingPersonnel.FullName = personnel.FullName;
                existingPersonnel.NumberId = personnel.NumberId;
                existingPersonnel.Gender = personnel.Gender;
                existingPersonnel.PhoneNumber = personnel.PhoneNumber;
                existingPersonnel.DateOfBirth = personnel.DateOfBirth;
                existingPersonnel.Address = personnel.Address;
                existingPersonnel.DateCreatedCccd = personnel.DateCreatedCccd;
                existingPersonnel.Status = (Status)personnel.Status;
                existingPersonnel.UpdatedAt = DateTime.UtcNow.AddHours(7);

                _context.Personnels.Update(existingPersonnel);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while updating personnel: {ex.Message}");
                throw;
            }
        }

        public async Task<Personnel?> GetPersonnelById(string id)
        {
            try
            {
                return await _context.Personnels.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting personnel by ID: {ex.Message}");
                throw;
            }
        }

        public async Task<Personnel?> GetPersonnelByNumberId(string numberId)
        {
            try
            {
                return await _context.Personnels
                    .FirstOrDefaultAsync(p => p.NumberId == numberId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting personnel by NumberId: {ex.Message}");
                throw;
            }
        }

        public IEnumerable<Personnel> GetAllPersonnel()
        {
            try
            {
                return _context.Personnels.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting all personnel: {ex.Message}");
                throw;
            }
        }

        private Result TryMultipleDecodingMethods(Bitmap bitmap)
        {
            // Phương pháp 1: Đọc chuẩn với cấu hình nâng cao
            var reader = new BarcodeReaderGeneric
            {
                AutoRotate = true,
                TryInverted = true,
                Options = {
            TryHarder = true,
            PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE },
            CharacterSet = "UTF-8"
        }
            };

            var result = reader.Decode(new BitmapLuminanceSource(bitmap));
            if (result != null) return result;

            // Phương pháp 2: Điều chỉnh độ phân giải và thử lại
            using (var resized = new Bitmap(bitmap, new System.DrawingCore.Size(bitmap.Width * 2, bitmap.Height * 2)))
            {
                result = reader.Decode(new BitmapLuminanceSource(resized));
                if (result != null) return result;
            }

            // Phương pháp 3: Thử với ảnh đen trắng
            using (var bwBitmap = ConvertToBlackAndWhite(bitmap))
            {
                result = reader.Decode(new BitmapLuminanceSource(bwBitmap));
                if (result != null) return result;
            }

            // Phương pháp 4: Mở rộng thử với các định dạng khác (nếu là mã khác QR)
            reader.Options.PossibleFormats = new List<BarcodeFormat> {
                BarcodeFormat.QR_CODE,
                BarcodeFormat.DATA_MATRIX,
                BarcodeFormat.PDF_417,
                BarcodeFormat.AZTEC
            };
            result = reader.Decode(new BitmapLuminanceSource(bitmap));

            return result;
        }

        private Bitmap ConvertToBlackAndWhite(Bitmap original)
        {
            Bitmap bwBitmap = new Bitmap(original.Width, original.Height);

            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    Color pixelColor = original.GetPixel(x, y);
                    int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
                    Color newColor = grayValue < 128 ? Color.Black : Color.White;
                    bwBitmap.SetPixel(x, y, newColor);
                }
            }

            return bwBitmap;
        }

        public async Task<Personnel?> GetInfoCccdByImage(IFormFile cccd, int options)
        {
            try
            {
                if (cccd == null)
                {
                    _logger.LogWarning("No CCCD image provided");
                    return null;
                }

                // Decode CCCD from barcode
                using MemoryStream ms = new MemoryStream();
                await cccd.CopyToAsync(ms);
                ms.Position = 0;

                using Bitmap bitmap = new Bitmap(ms);

                //// Convert Bitmap to LuminanceSource
                //var luminanceSource = new BitmapLuminanceSource(bitmap);

                //// Create a properly instantiated barcode reader
                //var reader = new BarcodeReaderGeneric();

                //var result = reader.Decode(luminanceSource);
                var result = TryMultipleDecodingMethods(bitmap);

                if (result != null)
                {
                    string[] decodedText = result.Text.Split("|");
                    //Console.WriteLine($"List text info of cccd {decodedText}");

                    if (decodedText.Length < 7)
                    {
                        _logger.LogWarning($"Invalid CCCD data format: {result.Text}");
                        return null;
                    }

                    var personnel = new Personnel
                    {
                        NumberId = decodedText[0],
                        FullName = decodedText[2],
                        DateOfBirth = decodedText[3],
                        Gender = decodedText[4],
                        Address = decodedText[5],
                        DateCreatedCccd = decodedText[6],
                        Id = ""
                        //RoleType = RoleType.Nhom_5
                    };

                    if (options == 1)
                    {
                        // Detect avatar from CCCD
                        IFormFile? avatarFile = await DetectAvatar.Detect(cccd);

                        if (avatarFile == null) return null;

                        // Upload avatar to Firebase Storage
                        var avatarUrl = await _firebaseService.UploadFileAsync(avatarFile, "avatars", personnel.NumberId);

                        // Set the avatar URL
                        personnel.AvatarUrl = avatarUrl.ToString();
                    }

                    return personnel;
                }
                else
                {
                    _logger.LogWarning("No barcode detected in the image");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while detecting CCCD: {ex.Message}");
                throw;
            }
        }

        public async Task<object> CheckAuthenByWebcam(WebSocket webSocket, CancellationToken cancellationToken)
        {
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

            while (!result.CloseStatus.HasValue)
            {
                // Process the received data (image or QR code data)
                string data = Encoding.UTF8.GetString(buffer, 0, result.Count);

                // If it's a QR code with a NumberId
                if (data.StartsWith("NUMBERID:"))
                {
                    string numberId = data.Substring(9);

                    // Process check-in/check-out
                    var personnel = await ProcessCheckInOut(numberId);

                    if (personnel != null)
                    {
                        // Send response back to client
                        var response = new
                        {
                            success = true,
                            message = personnel.Status == Status.CheckIn ? "Check-in successful" : "Check-out successful",
                            data = personnel
                        };

                        var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
                        await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, cancellationToken);

                        return personnel;
                    }
                    else
                    {
                        // Send error response
                        var response = new
                        {
                            success = false,
                            message = "Personnel not found"
                        };

                        var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
                        await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, cancellationToken);
                    }
                }

                // Receive next message
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
            }

            // Close the WebSocket connection
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, cancellationToken);

            return null;
        }

        public async Task<RolePersonnel> AddRolePersonnel(string roleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    throw new ArgumentException("Role name is required.");
                }

                var role = new RolePersonnel
                {
                    Id = Guid.NewGuid(),
                    RoleName = roleName,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7)
                };

                _context.RolePersonnels.Add(role);
                await _context.SaveChangesAsync();
                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while adding RolePersonnel: {ex.Message}");
                throw;
            }
        }

        public async Task<RolePersonnel> UpdateRolePersonnel(Guid roleId, string roleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    throw new ArgumentException("Role name is required.");
                }

                var role = await _context.RolePersonnels.FindAsync(roleId);
                if (role == null)
                {
                    _logger.LogWarning($"RolePersonnel with ID {roleId} not found");
                    throw new KeyNotFoundException($"RolePersonnel with ID {roleId} not found");
                }

                role.RoleName = roleName;
                role.UpdatedAt = DateTime.UtcNow.AddHours(7);

                _context.RolePersonnels.Update(role);
                await _context.SaveChangesAsync();
                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while updating RolePersonnel: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteRolePersonnel(Guid roleId)
        {
            try
            {
                var role = await _context.RolePersonnels
                    .Include(r => r.Personnels)
                    .FirstOrDefaultAsync(r => r.Id == roleId);

                if (role == null)
                {
                    _logger.LogWarning($"RolePersonnel with ID {roleId} not found");
                    return false;
                }

                // Gỡ mối quan hệ với các Personnel trước khi xóa
                if (role.Personnels.Any())
                {
                    foreach (var personnel in role.Personnels)
                    {
                        personnel.RolePersonnel = Guid.Empty;
                        personnel.RolePersonnels = null;
                    }
                }

                _context.RolePersonnels.Remove(role);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while deleting RolePersonnel: {ex.Message}");
                throw;
            }
        }

        public async Task<RolePersonnel?> GetRolePersonnelById(Guid roleId)
        {
            try
            {
                return await _context.RolePersonnels
                    .Include(r => r.Personnels)
                    .FirstOrDefaultAsync(r => r.Id == roleId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting RolePersonnel by ID: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<RolePersonnel>> GetAllRolePersonnel()
        {
            try
            {
                return await _context.RolePersonnels
                    .Include(r => r.Personnels)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting all RolePersonnel: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> AssignRolePersonnelToPersonnel(Guid personnelId, Guid roleId)
        {
            try
            {
                var personnel = await _context.Personnels.FindAsync(personnelId);
                if (personnel == null)
                {
                    _logger.LogWarning($"Personnel with ID {personnelId} not found");
                    return false;
                }

                var role = await _context.RolePersonnels.FindAsync(roleId);
                if (role == null)
                {
                    _logger.LogWarning($"RolePersonnel with ID {roleId} not found");
                    return false;
                }

                personnel.RolePersonnel = roleId;
                personnel.RolePersonnels = role;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while assigning RolePersonnel to Personnel: {ex.Message}");
                throw;
            }
        }
    }
}
