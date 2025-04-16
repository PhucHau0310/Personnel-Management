using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Identity.Client;
using PersonnelManagement.Data;
using PersonnelManagement.Models.DTOs;
using PersonnelManagement.Models.Entities;
using PersonnelManagement.Repositories.Interfaces;
using PersonnelManagement.Security;

namespace PersonnelManagement.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _context;

        public AccountController(IAccountRepository accountRepository, ILogger<AccountController> logger, ApplicationDbContext context)
        {
            _accountRepository = accountRepository;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAccounts()
        {
            var accounts = await _accountRepository.GetAccounts();
            if (accounts == null || !accounts.Any())
            {
                return NotFound(new
                {
                    Message = "Accounts not found in the system.",
                });
            }

            return Ok(new {
                Message = "Get accounts successfully.",
                Data = accounts
            });
        }

        [HttpPost]
        [RequirePermission("ADD_ACCOUNT")]
        public async Task<IActionResult> AddAccount([FromBody] AddAccountDto accountDto)
        {
            if (accountDto == null || string.IsNullOrWhiteSpace(accountDto.Username) || string.IsNullOrWhiteSpace(accountDto.Password))
            {
                return BadRequest(new { Message = "Username and password are required." });
            }

            var result = await _accountRepository.AddAccount(accountDto.Email, accountDto.Username, accountDto.Password, accountDto.RoleId);

            return result ? Ok(new
            {
                Message = "Add account successfully."
            }) : NotFound(new
            {
                Message = "Add account is failed."
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(Guid id)
        {
            var account = await _accountRepository.GetAccountById(id);
            if (account == null)
            {
                return NotFound(new
                {
                    Message = "Account not found."
                });
            }
            return Ok(new
            {
                Message = "Get account successfully.",
                Data = account
            });
        }

        [HttpDelete("{id}")]
        [RequirePermission("DELETE_ACCOUNT")]
        public IActionResult DeleteAccountById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new
                {
                    Message = "AccountId is required.",
                });
            }

            var result = _accountRepository.DeleteAccountById(id);
            return result ? Ok(new
            {
                Message = $"Delete account {id} successfully."
            }) : NotFound(new
            {
                Message = $"Delete account {id} is failed."
            });
        }

        [HttpPut("{id}")]
        [RequirePermission("EDIT_ACCOUNT")]
        public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] UpdateAccountDto account)
        {
            var channgeAccount = await _accountRepository.UpdateAccount(id, account.Email, account.Username, account.RoleAccountId);

            return channgeAccount ? Ok(new {
                Message = $"Update {id} account successfully."
            }) : NotFound(new
            {
                Message = $"Error while update account {id}."
            });
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email, string username)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new
                {
                    Message = "Email and username are required.",
                });
            }

            try
            {
                // Call the service to handle forgot password logic
                var result = await _accountRepository.ForgotPassword(email, username);

                if (!result)
                {
                    return NotFound(new
                    {
                        Message = "Email and username not found in the system.",
                    });
                }

                return Ok(new
                {
                    Message = "Password reset email sent successfully.",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while processing the request.",
                    Data = ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode(string code, string username)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(code))
            {
                return BadRequest(new
                {
                    Message = "Username and Code is required.",
                });
            }

            try
            {
                var result = await _accountRepository.VerifyCode(code, username);

                if (!result)
                {
                    return BadRequest(new
                    {
                        Message = "Code is invalid or expired",
                    });
                }

                return Ok(new
                {
                    Message = "Verify code is successfully.",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while processing the verfiy code.",
                    Data = ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(string newPass, string username)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(newPass))
            {
                return BadRequest(new
                {
                    Message = "Email and Newpass is required.",
                });
            }

            try
            {
                var result = await _accountRepository.ChangePass(newPass, username);

                if (!result)
                {
                    return NotFound(new
                    {
                        Message = "Not found username.",
                    });
                }

                return Ok(new
                {
                    Message = "Change pass is successfully.",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while processing the change pass.",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoleAccounts()
        {
            var roles = await _context.RoleAccounts
                 .Include(r => r.RolePermissions)
                 .ThenInclude(rp => rp.Permission)
                 .ToListAsync();

            if (roles == null || !roles.Any())
            {
                return NotFound(new
                {
                    Message = "Roles not found in the system."
                });
            }

            var roleAccountDtos = roles.Select(role => new
            {
                Id = role.Id,
                RoleName = role.RoleName,
                CreatedAt = role.CreatedAt,
                UpdatedAt = role.UpdatedAt,
                Permissions = role.RolePermissions.Select(rp => new
                {
                    Id = rp.Permission.Id,
                    Code = rp.Permission.Code,
                    Name = rp.Permission.Name
                }).ToList()
            }).ToList();

            return Ok(new {
                Message = "Get roles successfully.",
                Data = roleAccountDtos
            });
        }

        [HttpPost("roles")]
        [RequirePermission("ADD_ROLE_ACCOUNT")]
        public async Task<IActionResult> AddRole([FromBody] CreateRoleAccountDto createRoleDto)
        {
            if (string.IsNullOrWhiteSpace(createRoleDto.RoleName))
            {
                return BadRequest(new { Message = "Role name is required." });
            }

            var permissions = await _context.Permissions
                .Where(p => createRoleDto.PermissionIds.Contains(p.Id))
                .ToListAsync();

            if (permissions.Count != createRoleDto.PermissionIds.Count)
            {
                return BadRequest(new { Message = "One or more permission IDs are invalid." });
            }

            var role = new RoleAccount
            {
                Id = Guid.NewGuid(),
                RoleName = createRoleDto.RoleName,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                UpdatedAt = DateTime.UtcNow.AddHours(7)
            };

            role.RolePermissions = createRoleDto.PermissionIds.Select(permissionId => new RolePermission
            {
                Id = Guid.NewGuid(),
                RoleAccountId = role.Id,
                PermissionId = permissionId,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                UpdatedAt = DateTime.UtcNow.AddHours(7)
            }).ToList();

            _context.RoleAccounts.Add(role);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Role created successfully.",
                Data = new RoleAccountDto
                {
                    Id = role.Id,
                    RoleName = role.RoleName,
                    Permissions = role.RolePermissions.Select(rp => rp.Permission.Code).ToList()
                }
            });
        }

        [HttpDelete("roles/{roleId}")]
        [RequirePermission("DELETE_ROLE_ACCOUNT")]
        public async Task<IActionResult> DeleteRoleById(Guid roleId)
        {
            var role = await _context.RoleAccounts
                .Include(r => r.RolePermissions)
                .Include(r => r.Accounts)
                .FirstOrDefaultAsync(r => r.Id == roleId);

            if (role == null)
            {
                return NotFound(new { Message = "Role not found." });
            }

            // Nếu role đang được gán cho một hoặc nhiều account, gỡ mối quan hệ trước khi xóa
            if (role.Accounts.Any())
            {
                foreach (var account in role.Accounts)
                {
                    account.RoleAccountId = Guid.Empty;
                    account.RoleAccount = null;
                }
            }

            // Xóa các RolePermission liên quan
            _context.RolePermissions.RemoveRange(role.RolePermissions);

            // Xóa role
            _context.RoleAccounts.Remove(role);

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Role deleted successfully." });
        }

        [HttpPut("roles/{roleId}")]
        [RequirePermission("EDIT_ROLE_ACCOUNT")]
        public async Task<IActionResult> UpdateRole(Guid roleId, [FromBody] UpdateRoleAccountDto updateRoleDto)
        {
            if (string.IsNullOrWhiteSpace(updateRoleDto.RoleName))
            {
                return BadRequest(new { Message = "Role name is required." });
            }

            // Sử dụng execution strategy của DbContext
            var executionStrategy = _context.Database.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var role = await _context.RoleAccounts
                        .Include(r => r.RolePermissions)
                        .FirstOrDefaultAsync(r => r.Id == roleId);

                    if (role == null)
                    {
                        return NotFound(new { Message = "Role not found." });
                    }

                    // Cập nhật thông tin vai trò
                    role.RoleName = updateRoleDto.RoleName;
                    role.UpdatedAt = DateTime.UtcNow.AddHours(7);

                    // Xóa tất cả role permissions hiện tại
                    // Cách này đơn giản hơn để tránh các vấn đề về tracking entities
                    var existingPermissions = await _context.RolePermissions
                        .Where(rp => rp.RoleAccountId == roleId)
                        .ToListAsync();

                    _context.RolePermissions.RemoveRange(existingPermissions);
                    await _context.SaveChangesAsync();

                    // Thêm lại các permission mới
                    var permissions = await _context.Permissions
                        .Where(p => updateRoleDto.PermissionIds.Contains(p.Id))
                        .ToListAsync();

                    if (permissions.Count != updateRoleDto.PermissionIds.Count)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(new { Message = "One or more permission IDs are invalid." });
                    }

                    // Tạo và thêm các role permissions mới
                    var newRolePermissions = updateRoleDto.PermissionIds.Select(permissionId => new RolePermission
                    {
                        Id = Guid.NewGuid(),
                        RoleAccountId = role.Id,
                        PermissionId = permissionId,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    });

                    await _context.RolePermissions.AddRangeAsync(newRolePermissions);

                    // Cập nhật role và lưu thay đổi
                    _context.RoleAccounts.Update(role);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    // Load lại role với permissions mới để trả về response
                    var updatedRole = await _context.RoleAccounts
                        .Include(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                        .FirstOrDefaultAsync(r => r.Id == roleId);

                    return Ok(new
                    {
                        Message = "Role updated successfully.",
                        Data = new RoleAccountDto
                        {
                            Id = updatedRole.Id,
                            RoleName = updatedRole.RoleName,
                            Permissions = updatedRole.RolePermissions
                                .Select(rp => rp.Permission?.Code)
                                .Where(code => code != null)
                                .ToList()
                        }
                    });
                }
                catch (DbUpdateConcurrencyException)
                {
                    await transaction.RollbackAsync();
                    return Conflict(new { Message = "Concurrency conflict: The role may have been modified or deleted." });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, new { Message = $"An error occurred: {ex.Message}" });
                }
            });
        }

        // Phương thức mới để gán RoleAccount cho Account
        [HttpPost("{accountId}/assign-role/{roleId}")]
        [RequirePermission("EDIT_ACCOUNT")]
        public async Task<IActionResult> AssignRoleToAccount(Guid accountId, Guid roleId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
            {
                return NotFound(new { Message = "Account not found." });
            }

            var role = await _context.RoleAccounts.FindAsync(roleId);
            if (role == null)
            {
                return NotFound(new { Message = "Role not found." });
            }

            account.RoleAccountId = roleId;
            account.RoleAccount = role;

            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Role {role.RoleName} assigned to account {account.Username} successfully." });
        }

        [HttpGet("permissions")]
        public async Task<IActionResult> GetAllPermissions()
        {
            try
            {
                var permissions = await _context.Permissions
                    .Include(p => p.RolePermissions)
                        .ThenInclude(rp => rp.RoleAccount)
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                if (permissions == null || !permissions.Any())
                {
                    return NotFound(new
                    {
                        Message = "Permissions not found in the system."
                    });
                }

                var permissionDtos = permissions.Select(p => new
                {
                    Id = p.Id,
                    Name = p.Name,
                    Code = p.Code,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    RolePermissions = p.RolePermissions.Select(rp => new
                    {
                        Id = rp.Id,
                        RoleId = rp.RoleAccountId,
                        RoleName = rp.RoleAccount != null ? rp.RoleAccount.RoleName : null,
                        CreatedAt = rp.CreatedAt,
                        UpdatedAt = rp.UpdatedAt
                    }).ToList()
                }).ToList();

                return Ok(new
                {
                    Message = "Get permissions successfully.",
                    Data = permissionDtos
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving permissions");
                return StatusCode(500, new
                {
                    Message = "An error occurred while retrieving permissions.",
                    Error = ex.Message
                });
            }
        }
    }
}