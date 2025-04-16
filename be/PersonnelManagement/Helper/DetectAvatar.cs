//using System.Data.Entity.Core.Metadata.Edm;
//using System.Drawing;
//using System;
//using Emgu.CV;
//using Emgu.CV.Structure;
//using Emgu.CV.Util;
//using Emgu.CV.CvEnum;

//namespace PersonnelManagement.Helper
//{
//    public class DetectAvatar
//    {
//        public static async Task<IFormFile?> Detect(IFormFile imageFile)
//        {
//            try
//            {
//                // Tạo một tệp tạm thời để lưu hình ảnh đã tải lên
//                string tempFilePath = Path.GetTempFileName() + ".jpg";

//                using (var stream = new FileStream(tempFilePath, FileMode.Create))
//                {
//                    await imageFile.CopyToAsync(stream);
//                }

//                if (!File.Exists(tempFilePath))
//                {
//                    Console.WriteLine("Không thể tạo file tạm thời: " + Path.GetFullPath(tempFilePath));
//                    return null;
//                }

//                // Load model phát hiện khuôn mặt (OpenCV)
//                //string haarcascadePath = "haarcascade_frontalface_default.xml";
//                string haarcascadePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Helper", "haarcascade_frontalface_default.xml");


//                // Load ảnh gốc
//                Mat image = CvInvoke.Imread(tempFilePath, ImreadModes.Color);
//                if (image.IsEmpty)
//                {
//                    Console.WriteLine("Không thể mở ảnh.");
//                    return null;
//                }

//                // Tạo bản sao để hiển thị kết quả
//                Mat outputImage = image.Clone();

//                // Phương pháp 1: Tập trung vào vùng bên trái của CCCD
//                // Định nghĩa vùng quan tâm (ROI) ở bên trái ảnh, chiếm khoảng 1/3 chiều rộng
//                int roiWidth = image.Width / 3;
//                Rectangle leftRegion = new Rectangle(0, 0, roiWidth, image.Height);

//                // Cắt vùng ROI
//                Mat leftROI = new Mat(image, leftRegion);

//                // Chuyển ảnh ROI sang grayscale
//                Mat gray = new Mat();
//                CvInvoke.CvtColor(leftROI, gray, ColorConversion.Bgr2Gray);

//                // Áp dụng các kỹ thuật xử lý ảnh để tăng độ tương phản
//                CvInvoke.EqualizeHist(gray, gray);

//                // Phát hiện khuôn mặt trong vùng ROI
//                CascadeClassifier faceDetector = new CascadeClassifier(haarcascadePath);
//                var faces = faceDetector.DetectMultiScale(gray, 1.1, 5, new Size(20, 20));

//                // Biến để lưu vùng avatar được phát hiện
//                Rectangle avatarRect = Rectangle.Empty;

//                if (faces.Length > 0)
//                {
//                    // Giả định khuôn mặt đầu tiên là khuôn mặt trong avatar
//                    var face = faces[0];

//                    // Điều chỉnh tọa độ để phù hợp với ảnh gốc
//                    face.X += leftRegion.X;
//                    face.Y += leftRegion.Y;

//                    // Dùng khuôn mặt để ước tính vùng avatar 3x4
//                    // Ước tính tỷ lệ: ảnh 3x4 thường có chiều cao gấp 4/3 chiều rộng
//                    int faceExpansion = (int)(face.Width * 0.4); // Mở rộng để lấy toàn bộ avatar

//                    avatarRect = new Rectangle(
//                        Math.Max(0, face.X - faceExpansion),
//                        Math.Max(0, face.Y - faceExpansion),
//                        (int)(face.Width + faceExpansion * 2),
//                        (int)((face.Width + faceExpansion * 2) * 4.0 / 3.0)
//                    );

//                    // Đảm bảo rectangle nằm trong ảnh
//                    avatarRect.Width = Math.Min(image.Width - avatarRect.X, avatarRect.Width);
//                    avatarRect.Height = Math.Min(image.Height - avatarRect.Y, avatarRect.Height);
//                }
//                else
//                {
//                    // Nếu không phát hiện được khuôn mặt, thử phương pháp tìm kiếm contour
//                    Mat edges = new Mat();
//                    CvInvoke.Canny(gray, edges, 50, 150);

//                    using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
//                    {
//                        CvInvoke.FindContours(edges, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

//                        // Tìm các contour hình chữ nhật có tỷ lệ gần với 3:4
//                        double targetRatio = 3.0 / 4.0; // Tỷ lệ ảnh 3x4
//                        double bestRatioDiff = double.MaxValue;
//                        Rectangle bestRect = Rectangle.Empty;

//                        for (int i = 0; i < contours.Size; i++)
//                        {
//                            using (VectorOfPoint approx = new VectorOfPoint())
//                            {
//                                CvInvoke.ApproxPolyDP(contours[i], approx,
//                                    CvInvoke.ArcLength(contours[i], true) * 0.02, true);

//                                // Kiểm tra nếu là hình chữ nhật (4 đỉnh)
//                                if (approx.Size == 4)
//                                {
//                                    Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);

//                                    // Bỏ qua các hình chữ nhật quá nhỏ
//                                    if (rect.Width < 40 || rect.Height < 50)
//                                        continue;

//                                    // Kiểm tra tỷ lệ kích thước
//                                    double ratio = (double)rect.Height / rect.Width;
//                                    double ratioDiff = Math.Abs(ratio - targetRatio);

//                                    // Tìm hình chữ nhật có tỷ lệ gần với 3:4 nhất
//                                    if (ratioDiff < bestRatioDiff)
//                                    {
//                                        bestRatioDiff = ratioDiff;
//                                        bestRect = rect;
//                                    }
//                                }
//                            }
//                        }

//                        // Nếu tìm thấy hình chữ nhật phù hợp
//                        if (bestRect != Rectangle.Empty && bestRatioDiff < 0.3)
//                        {
//                            // Điều chỉnh tọa độ để phù hợp với ảnh gốc
//                            bestRect.X += leftRegion.X;
//                            bestRect.Y += leftRegion.Y;
//                            avatarRect = bestRect;
//                        }
//                    }
//                }

//                // Nếu không tìm thấy avatar bằng cả hai phương pháp, thử phương pháp cuối: 
//                // giả định avatar nằm ở góc trái phía trên với tỷ lệ 3:4
//                if (avatarRect.IsEmpty)
//                {
//                    // Giả định avatar chiếm khoảng 1/4 chiều cao của CCCD
//                    int estimatedAvatarHeight = image.Height / 4;
//                    int estimatedAvatarWidth = (int)(estimatedAvatarHeight * 3.0 / 4.0);

//                    // Giả định avatar nằm ở góc trái trên với một số padding
//                    int paddingX = image.Width / 50; // 2% chiều rộng
//                    int paddingY = image.Height / 25; // 4% chiều cao

//                    avatarRect = new Rectangle(
//                        paddingX,
//                        paddingY,
//                        estimatedAvatarWidth,
//                        estimatedAvatarHeight
//                    );
//                }

//                // Vẽ hình chữ nhật xung quanh avatar đã phát hiện
//                //CvInvoke.Rectangle(outputImage, avatarRect, new MCvScalar(0, 255, 0), 2);

//                // Cắt vùng avatar
//                Mat avatarImage = new Mat(image, avatarRect);

//                // Lưu ảnh avatar
//                //CvInvoke.Imwrite("avatar_3x4.jpg", avatarImage);

//                // Hiển thị ảnh avatar và ảnh gốc có đánh dấu
//                //CvInvoke.Imshow("Avatar 3x4", avatarImage);
//                //CvInvoke.Imshow("CCCD with Detected Avatar", outputImage);
//                //CvInvoke.WaitKey(0);

//                // Lưu ảnh avatar vào một file tạm
//                string avatarTempPath = Path.GetTempFileName() + ".jpg";
//                CvInvoke.Imwrite(avatarTempPath, avatarImage);

//                // Tạo IFormFile từ file ảnh avatarTempPath
//                var fileStream = new FileStream(avatarTempPath, FileMode.Open, FileAccess.Read);
//                var formFile = new FormFile(
//                    fileStream,
//                    0,
//                    fileStream.Length,
//                    "avatar",
//                    Path.GetFileName(avatarTempPath))
//                {
//                    Headers = new HeaderDictionary(),
//                    ContentType = "image/jpeg"
//                };

//                // Xóa file tạm
//                File.Delete(tempFilePath);

//                return formFile;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Lỗi khi nhận diện ảnh: " + ex.Message);
//                if (ex.InnerException != null)
//                {
//                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
//                    Console.WriteLine($"Stack trace: {ex.InnerException.StackTrace}");
//                }
//                return null;
//            }
//        }
//    }
//}
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace PersonnelManagement.Helper
{
    public class DetectAvatar
    {
        public static async Task<IFormFile?> Detect(IFormFile imageFile)
        {
            //try
            //{
            //    // Tạo một tệp tạm thời để lưu hình ảnh đã tải lên
            //    string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".jpg");
            //    string avatarTempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".jpg");

            //    try
            //    {
            //        // Lưu ảnh CCCD gốc vào file tạm
            //        using (var stream = new FileStream(tempFilePath, FileMode.Create))
            //        {
            //            await imageFile.CopyToAsync(stream);
            //        }

            //        if (!File.Exists(tempFilePath))
            //        {
            //            Console.WriteLine("Không thể tạo file tạm thời: " + Path.GetFullPath(tempFilePath));
            //            return null;
            //        }

            //        // Xử lý ảnh bằng ImageSharp
            //        using (var image = await Image.LoadAsync(tempFilePath))
            //        {
            //            // Xác định vị trí avatar trên CCCD Việt Nam
            //            // Ảnh nằm ở góc trái dưới với tỷ lệ 3x4

            //            // Tùy chỉnh các tham số này theo đặc điểm của CCCD Việt Nam
            //            int leftPadding = image.Width / 42;    // Khoảng cách từ mép trái

            //            // Avatar nằm ở góc trái dưới, tính toán vị trí từ dưới lên
            //            int bottomPadding = image.Height / 20; // Khoảng cách từ mép dưới

            //            // Avatar thường chiếm khoảng 1/4 chiều rộng của CCCD
            //            int avatarWidth = image.Width / 4;

            //            // Với tỷ lệ 3x4 (rộng:cao)
            //            int avatarHeight = (int)(avatarWidth * 5.5 / 3.0);

            //            // Tính toán vị trí top dựa vào chiều cao ảnh và bottomPadding
            //            int topPadding = image.Height - bottomPadding - avatarHeight;

            //            // Đảm bảo kích thước nằm trong giới hạn ảnh
            //            avatarWidth = Math.Min(avatarWidth, image.Width - leftPadding);
            //            avatarHeight = Math.Min(avatarHeight, image.Height - topPadding);

            //            // Cắt phần avatar từ ảnh gốc
            //            using (var avatar = image.Clone(ctx => ctx.Crop(
            //                new Rectangle(leftPadding, topPadding, avatarWidth, avatarHeight))))
            //            {
            //                // Lưu avatar vào file tạm
            //                await avatar.SaveAsync(avatarTempPath, new JpegEncoder());
            //            }
            //        }

            //        // Tạo IFormFile từ file ảnh avatarTempPath
            //        var fileStream = new FileStream(avatarTempPath, FileMode.Open, FileAccess.Read);
            //        var formFile = new FormFile(
            //            fileStream,
            //            0,
            //            fileStream.Length,
            //            "avatar",
            //            Path.GetFileName(avatarTempPath))
            //        {
            //            Headers = new HeaderDictionary(),
            //            ContentType = "image/jpeg"
            //        };

            //        return formFile;
            //    }
            //    finally
            //    {
            //        // Xóa file tạm khi hoàn tất
            //        if (File.Exists(tempFilePath))
            //        {
            //            try { File.Delete(tempFilePath); } catch { /* ignore cleanup errors */ }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Lỗi khi cắt ảnh từ CCCD: " + ex.Message);
            //    if (ex.InnerException != null)
            //    {
            //        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            //        Console.WriteLine($"Stack trace: {ex.InnerException.StackTrace}");
            //    }
            //    return null;
            //}
            try
            {
                // Tạo một tệp tạm thời để lưu hình ảnh đã tải lên
                string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".jpg");
                string avatarTempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".jpg");

                try
                {
                    // Lưu ảnh CCCD gốc vào file tạm
                    using (var stream = new FileStream(tempFilePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    if (!File.Exists(tempFilePath))
                    {
                        Console.WriteLine("Không thể tạo file tạm thời: " + Path.GetFullPath(tempFilePath));
                        return null;
                    }

                    // Xử lý ảnh bằng ImageSharp
                    using (var image = await Image.LoadAsync(tempFilePath))
                    {
                        // Phương pháp mới: Cắt ảnh từ góc trái dưới với chiều dọc dài hơn

                        // Điều chỉnh các tham số này theo đặc điểm của CCCD Việt Nam mới
                        double leftRatio = 0.025;           // Tỷ lệ khoảng cách từ mép trái (2.5%)
                        double bottomRatio = 0.08;          // Tỷ lệ khoảng cách từ mép dưới (8%)
                        double widthRatio = 0.26;           // Tỷ lệ chiều rộng so với ảnh gốc (26%)
                        double heightRatio = 0.45;          // Tỷ lệ chiều cao so với ảnh gốc (45%)

                        int leftPadding = (int)(image.Width * leftRatio);
                        int bottomPadding = (int)(image.Height * bottomRatio);
                        int avatarWidth = (int)(image.Width * widthRatio);
                        int avatarHeight = (int)(image.Height * heightRatio);

                        // Tính toán vị trí top từ dưới lên
                        int topPadding = image.Height - bottomPadding - avatarHeight;

                        // Đảm bảo kích thước và vị trí nằm trong giới hạn ảnh
                        avatarWidth = Math.Min(avatarWidth, image.Width - leftPadding);
                        topPadding = Math.Max(0, topPadding);
                        avatarHeight = Math.Min(avatarHeight, image.Height - topPadding);

                        // Cắt phần avatar từ ảnh gốc
                        using (var avatar = image.Clone(ctx => ctx.Crop(
                            new Rectangle(leftPadding, topPadding, avatarWidth, avatarHeight))))
                        {
                            // Lưu avatar vào file tạm
                            await avatar.SaveAsync(avatarTempPath, new JpegEncoder());
                        }
                    }

                    // Tạo IFormFile từ file ảnh avatarTempPath
                    var fileStream = new FileStream(avatarTempPath, FileMode.Open, FileAccess.Read);
                    var formFile = new FormFile(
                        fileStream,
                        0,
                        fileStream.Length,
                        "avatar",
                        Path.GetFileName(avatarTempPath))
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "image/jpeg"
                    };

                    return formFile;
                }
                finally
                {
                    // Xóa file tạm khi hoàn tất
                    if (File.Exists(tempFilePath))
                    {
                        try { File.Delete(tempFilePath); } catch { /* ignore cleanup errors */ }
                    }
                    // Chú ý: File avatarTempPath sẽ được xóa khi FormFile được dispose
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi cắt ảnh từ CCCD: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Stack trace: {ex.InnerException.StackTrace}");
                }
                return null;
            }
        }

        // Phương thức cải tiến cho trường hợp biết chính xác vị trí avatar trên CCCD
        public static async Task<IFormFile?> DetectWithFixedPosition(IFormFile imageFile,
            double leftPercentage = 0.025, double bottomPercentage = 0.05,
            double widthPercentage = 0.25, double heightPercentage = 0.38)
        {
            try
            {
                // Tạo một tệp tạm thời để lưu hình ảnh đã tải lên
                string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".jpg");
                string avatarTempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".jpg");

                try
                {
                    // Lưu ảnh CCCD gốc vào file tạm
                    using (var stream = new FileStream(tempFilePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    if (!File.Exists(tempFilePath))
                    {
                        Console.WriteLine("Không thể tạo file tạm thời: " + Path.GetFullPath(tempFilePath));
                        return null;
                    }

                    // Xử lý ảnh bằng ImageSharp
                    using (var image = await Image.LoadAsync(tempFilePath))
                    {
                        // Tính toán vị trí và kích thước dựa trên tỷ lệ % của ảnh gốc
                        int leftPadding = (int)(image.Width * leftPercentage);
                        int avatarWidth = (int)(image.Width * widthPercentage);
                        int avatarHeight = (int)(image.Height * heightPercentage);
                        int bottomPadding = (int)(image.Height * bottomPercentage);

                        // Tính toán vị trí top từ chiều cao ảnh, chiều cao avatar và bottomPadding
                        int topPadding = image.Height - bottomPadding - avatarHeight;

                        // Đảm bảo kích thước và vị trí nằm trong giới hạn ảnh
                        avatarWidth = Math.Min(avatarWidth, image.Width - leftPadding);
                        topPadding = Math.Max(0, topPadding); // Đảm bảo không âm
                        avatarHeight = Math.Min(avatarHeight, image.Height - topPadding);

                        // Cắt phần avatar từ ảnh gốc
                        using (var avatar = image.Clone(ctx => ctx.Crop(
                            new Rectangle(leftPadding, topPadding, avatarWidth, avatarHeight))))
                        {
                            // Lưu avatar vào file tạm
                            await avatar.SaveAsync(avatarTempPath, new JpegEncoder());
                        }
                    }

                    // Tạo IFormFile từ file ảnh avatarTempPath
                    var fileStream = new FileStream(avatarTempPath, FileMode.Open, FileAccess.Read);
                    var formFile = new FormFile(
                        fileStream,
                        0,
                        fileStream.Length,
                        "avatar",
                        Path.GetFileName(avatarTempPath))
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "image/jpeg"
                    };

                    return formFile;
                }
                finally
                {
                    // Xóa file tạm khi hoàn tất
                    if (File.Exists(tempFilePath))
                    {
                        try { File.Delete(tempFilePath); } catch { /* ignore cleanup errors */ }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi cắt ảnh từ CCCD: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Stack trace: {ex.InnerException.StackTrace}");
                }
                return null;
            }
        }

        // Phương thức tiện ích để xử lý ảnh trong Stream trực tiếp
        public static async Task<MemoryStream> DetectAvatarToStreamAsync(Stream inputStream,
            double leftPercentage = 0.025, double bottomPercentage = 0.05,
            double widthPercentage = 0.25, double heightPercentage = 0.33)
        {
            try
            {
                // Đọc ảnh từ stream đầu vào
                using (var image = await Image.LoadAsync(inputStream))
                {
                    // Tính toán vị trí và kích thước dựa trên tỷ lệ % của ảnh gốc
                    int leftPadding = (int)(image.Width * leftPercentage);
                    int avatarWidth = (int)(image.Width * widthPercentage);
                    int avatarHeight = (int)(image.Height * heightPercentage);
                    int bottomPadding = (int)(image.Height * bottomPercentage);

                    // Tính vị trí top từ dưới lên
                    int topPadding = image.Height - bottomPadding - avatarHeight;

                    // Đảm bảo kích thước nằm trong giới hạn ảnh
                    avatarWidth = Math.Min(avatarWidth, image.Width - leftPadding);
                    topPadding = Math.Max(0, topPadding); // Đảm bảo không âm
                    avatarHeight = Math.Min(avatarHeight, image.Height - topPadding);

                    // Cắt phần avatar từ ảnh gốc
                    using (var avatar = image.Clone(ctx => ctx.Crop(
                        new Rectangle(leftPadding, topPadding, avatarWidth, avatarHeight))))
                    {
                        // Khởi tạo memory stream để lưu kết quả
                        var outputStream = new MemoryStream();

                        // Lưu avatar vào stream
                        await avatar.SaveAsync(outputStream, new JpegEncoder());

                        // Reset vị trí để đọc từ đầu
                        outputStream.Position = 0;
                        return outputStream;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi cắt ảnh từ stream: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Stack trace: {ex.InnerException.StackTrace}");
                }
                throw;
            }
        }
    }
}
//using System;
//using System.IO;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using OpenCvSharp;

//namespace PersonnelManagement.Helper
//{
//    public class DetectAvatar
//    {
//        public static async Task<IFormFile?> Detect(IFormFile imageFile)
//        {
//            try
//            {
//                // Tạo một tệp tạm thời để lưu hình ảnh đã tải lên
//                string tempFilePath = Path.GetTempFileName() + ".jpg";

//                using (var stream = new FileStream(tempFilePath, FileMode.Create))
//                {
//                    await imageFile.CopyToAsync(stream);
//                }

//                if (!File.Exists(tempFilePath))
//                {
//                    Console.WriteLine("Không thể tạo file tạm thời: " + Path.GetFullPath(tempFilePath));
//                    return null;
//                }

//                // Load model phát hiện khuôn mặt (OpenCV)
//                string haarcascadePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Helper", "haarcascade_frontalface_default.xml");
//                if (!File.Exists(haarcascadePath))
//                {
//                    Console.WriteLine($"Không tìm thấy file haar cascade: {haarcascadePath}");
//                    return null;
//                }

//                // Load ảnh gốc
//                using var image = Cv2.ImRead(tempFilePath, ImreadModes.Color);
//                if (image.Empty())
//                {
//                    Console.WriteLine("Không thể mở ảnh.");
//                    return null;
//                }

//                // Tạo bản sao để hiển thị kết quả
//                using var outputImage = image.Clone();

//                // Phương pháp 1: Tập trung vào vùng bên trái của CCCD
//                // Định nghĩa vùng quan tâm (ROI) ở bên trái ảnh, chiếm khoảng 1/3 chiều rộng
//                int roiWidth = image.Width / 3;
//                Rect leftRegion = new Rect(0, 0, roiWidth, image.Height);

//                // Cắt vùng ROI
//                using var leftROI = new Mat(image, leftRegion);

//                // Chuyển ảnh ROI sang grayscale
//                using var gray = new Mat();
//                Cv2.CvtColor(leftROI, gray, ColorConversionCodes.BGR2GRAY);

//                // Áp dụng các kỹ thuật xử lý ảnh để tăng độ tương phản
//                Cv2.EqualizeHist(gray, gray);

//                // Phát hiện khuôn mặt trong vùng ROI
//                using var faceDetector = new CascadeClassifier(haarcascadePath);
//                var faces = faceDetector.DetectMultiScale(
//                    gray,
//                    1.1,
//                    5,
//                    HaarDetectionTypes.DoCannyPruning,
//                    new Size(20, 20)
//                );

//                // Biến để lưu vùng avatar được phát hiện
//                Rect avatarRect = new Rect();
//                bool avatarFound = false;

//                if (faces.Length > 0)
//                {
//                    // Giả định khuôn mặt đầu tiên là khuôn mặt trong avatar
//                    var face = faces[0];

//                    // Điều chỉnh tọa độ để phù hợp với ảnh gốc
//                    face.X += leftRegion.X;
//                    face.Y += leftRegion.Y;

//                    // Dùng khuôn mặt để ước tính vùng avatar 3x4
//                    // Ước tính tỷ lệ: ảnh 3x4 thường có chiều cao gấp 4/3 chiều rộng
//                    int faceExpansion = (int)(face.Width * 0.4); // Mở rộng để lấy toàn bộ avatar

//                    avatarRect = new Rect(
//                        Math.Max(0, face.X - faceExpansion),
//                        Math.Max(0, face.Y - faceExpansion),
//                        (int)(face.Width + faceExpansion * 2),
//                        (int)((face.Width + faceExpansion * 2) * 4.0 / 3.0)
//                    );

//                    // Đảm bảo rectangle nằm trong ảnh
//                    avatarRect.Width = Math.Min(image.Width - avatarRect.X, avatarRect.Width);
//                    avatarRect.Height = Math.Min(image.Height - avatarRect.Y, avatarRect.Height);
//                    avatarFound = true;
//                }
//                else
//                {
//                    // Nếu không phát hiện được khuôn mặt, thử phương pháp tìm kiếm contour
//                    using var edges = new Mat();
//                    Cv2.Canny(gray, edges, 50, 150);

//                    Point[][] contours;
//                    HierarchyIndex[] hierarchy;
//                    Cv2.FindContours(edges, out contours, out hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);

//                    // Tìm các contour hình chữ nhật có tỷ lệ gần với 3:4
//                    double targetRatio = 3.0 / 4.0; // Tỷ lệ ảnh 3x4
//                    double bestRatioDiff = double.MaxValue;
//                    Rect bestRect = new Rect();

//                    foreach (var contour in contours)
//                    {
//                        // Xấp xỉ contour thành đa giác
//                        var approx = Cv2.ApproxPolyDP(contour,
//                            Cv2.ArcLength(contour, true) * 0.02, true);

//                        // Kiểm tra nếu là hình chữ nhật (4 đỉnh)
//                        if (approx.Length == 4)
//                        {
//                            Rect rect = Cv2.BoundingRect(contour);

//                            // Bỏ qua các hình chữ nhật quá nhỏ
//                            if (rect.Width < 40 || rect.Height < 50)
//                                continue;

//                            // Kiểm tra tỷ lệ kích thước
//                            double ratio = (double)rect.Height / rect.Width;
//                            double ratioDiff = Math.Abs(ratio - targetRatio);

//                            // Tìm hình chữ nhật có tỷ lệ gần với 3:4 nhất
//                            if (ratioDiff < bestRatioDiff)
//                            {
//                                bestRatioDiff = ratioDiff;
//                                bestRect = rect;
//                            }
//                        }
//                    }

//                    // Nếu tìm thấy hình chữ nhật phù hợp
//                    if (bestRect.Width > 0 && bestRect.Height > 0 && bestRatioDiff < 0.3)
//                    {
//                        // Điều chỉnh tọa độ để phù hợp với ảnh gốc
//                        bestRect.X += leftRegion.X;
//                        bestRect.Y += leftRegion.Y;
//                        avatarRect = bestRect;
//                        avatarFound = true;
//                    }
//                }

//                // Nếu không tìm thấy avatar bằng cả hai phương pháp, thử phương pháp cuối: 
//                // giả định avatar nằm ở góc trái phía trên với tỷ lệ 3:4
//                if (!avatarFound)
//                {
//                    // Giả định avatar chiếm khoảng 1/4 chiều cao của CCCD
//                    int estimatedAvatarHeight = image.Height / 4;
//                    int estimatedAvatarWidth = (int)(estimatedAvatarHeight * 3.0 / 4.0);

//                    // Giả định avatar nằm ở góc trái trên với một số padding
//                    int paddingX = image.Width / 50; // 2% chiều rộng
//                    int paddingY = image.Height / 25; // 4% chiều cao

//                    avatarRect = new Rect(
//                        paddingX,
//                        paddingY,
//                        estimatedAvatarWidth,
//                        estimatedAvatarHeight
//                    );
//                }

//                // Cắt vùng avatar
//                using var avatarImage = new Mat(image, avatarRect);

//                // Lưu ảnh avatar vào một file tạm
//                string avatarTempPath = Path.GetTempFileName() + ".jpg";
//                avatarImage.SaveImage(avatarTempPath);

//                // Tạo IFormFile từ file ảnh avatarTempPath
//                var fileStream = new FileStream(avatarTempPath, FileMode.Open, FileAccess.Read);
//                var formFile = new FormFile(
//                    fileStream,
//                    0,
//                    fileStream.Length,
//                    "avatar",
//                    Path.GetFileName(avatarTempPath))
//                {
//                    Headers = new HeaderDictionary(),
//                    ContentType = "image/jpeg"
//                };

//                // Xóa file tạm
//                File.Delete(tempFilePath);

//                return formFile;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Lỗi khi nhận diện ảnh: " + ex.Message);
//                if (ex.InnerException != null)
//                {
//                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
//                    Console.WriteLine($"Stack trace: {ex.InnerException.StackTrace}");
//                }
//                return null;
//            }
//        }
//    }
//}

