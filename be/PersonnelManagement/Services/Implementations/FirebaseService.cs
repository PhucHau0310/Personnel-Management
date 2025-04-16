//using FirebaseAdmin;
//using Google.Apis.Auth.OAuth2;
//using Google.Cloud.Storage.V1;
//using PersonnelManagement.Services.Interfaces;

//namespace PersonnelManagement.Services.Implementations
//{
//    public class FirebaseService : IFirebaseService
//    {
//        private readonly StorageClient _storageClient;
//        private readonly string _bucketName;
//        private readonly ILogger<FirebaseService> _logger;

//        public FirebaseService(IConfiguration configuration, ILogger<FirebaseService> logger)
//        {
//            _logger = logger;

//            try
//            {
//                // Get the Firebase section first
//                var firebaseSection = configuration.GetSection("Firebase");
//                if (!firebaseSection.Exists())
//                {
//                    _logger.LogError("Firebase configuration section not found");
//                    throw new ArgumentException("Firebase configuration section not found");
//                }

//                var credentialsPath = firebaseSection.GetValue<string>("Credentials");
//                if (string.IsNullOrEmpty(credentialsPath))
//                {
//                    _logger.LogError("Firebase credentials path is missing in configuration");
//                    throw new ArgumentException("Firebase credentials path is missing in configuration.");
//                }

//                credentialsPath = Path.Combine(AppContext.BaseDirectory, credentialsPath);
//                _logger.LogInformation("Loading Firebase credentials from: {Path}", credentialsPath);

//                if (!File.Exists(credentialsPath))
//                {
//                    _logger.LogError("Firebase credentials file not found at: {Path}", credentialsPath);
//                    throw new FileNotFoundException($"Firebase credentials file not found at path: {credentialsPath}");
//                }

//                var credential = GoogleCredential.FromFile(credentialsPath);
//                _logger.LogInformation("Successfully loaded Firebase credentials");

//                if (FirebaseApp.DefaultInstance == null)
//                {
//                    FirebaseApp.Create(new AppOptions
//                    {
//                        Credential = credential
//                    });
//                    _logger.LogInformation("Created new Firebase App instance");
//                }

//                _storageClient = StorageClient.Create(credential);
//                if (_storageClient == null)
//                {
//                    _logger.LogError("StorageClient is null. Ensure Firebase credentials are correct.");
//                    throw new NullReferenceException("StorageClient is not initialized.");
//                }

//                _bucketName = configuration["Firebase:StorageBucket"];

//                if (string.IsNullOrEmpty(_bucketName))
//                {
//                    _logger.LogError("Firebase storage bucket name is missing in configuration");
//                    throw new ArgumentException("Firebase storage bucket name is missing in configuration");
//                }

//                _logger.LogInformation("Firebase service initialized successfully with bucket: {BucketName}", _bucketName);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error initializing Firebase service");
//                throw;
//            }

//        }

//        public async Task<string> UploadFileAsync(IFormFile file, string folder, string numberId)
//        {
//            try
//            {
//                if (file == null)
//                {
//                    _logger.LogError("File is null in UploadFileAsync.");
//                    throw new ArgumentNullException(nameof(file), "File cannot be null.");
//                }

//                // Generate unique filename
//                var fileName = $"{folder}/avatar-{numberId}{Path.GetExtension(file.FileName)}";

//                using var memoryStream = new MemoryStream();
//                await file.CopyToAsync(memoryStream);
//                memoryStream.Position = 0;

//                // Upload to Firebase Storage
//                var dataObject = await _storageClient.UploadObjectAsync(
//                    _bucketName,
//                    fileName,
//                    file.ContentType,
//                    memoryStream,
//                    new UploadObjectOptions { PredefinedAcl = PredefinedObjectAcl.PublicRead }); // Public Read

//                // Return public URL
//                return $"https://storage.googleapis.com/{_bucketName}/{fileName}";
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("File upload failed", ex);
//            }
//        }

//        public async Task DeleteFileAsync(string fileUrl)
//        {
//            try
//            {
//                _logger.LogInformation("Attempting to delete file: {FileUrl}", fileUrl);

//                // Extract object name from URL
//                var uri = new Uri(fileUrl);
//                var objectName = uri.LocalPath.TrimStart('/');
//                objectName = objectName.Substring(objectName.IndexOf('/') + 1);

//                _logger.LogInformation("Extracted object name: {ObjectName}", objectName);

//                // Delete from Firebase Storage  
//                await _storageClient.DeleteObjectAsync(_bucketName, objectName);

//                _logger.LogInformation("Successfully deleted file");
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "File deletion failed");
//                throw new Exception("File deletion failed", ex);
//            }
//        }

//        public async Task<string> GetFileUrlAsync(string path)
//        {
//            //var storage = storageclient.create();
//            //var bucket = storage.getbucket("e-commerce-c9b1d.appspot.com");

//            //tạo signed url có thời hạn
//            //var signedurl = await storage.getsignedurlasync(
//            //    bucket.name,
//            //    path,
//            //timespan.fromhours(1), // url có hiệu lực trong 1 giờ
//            //    signurloptions.fromcredential(credential)
//            //);

//            //return signedurl;
//            throw new Exception("mm");
//        }
//    }
//}


using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using PersonnelManagement.Services.Interfaces;

namespace PersonnelManagement.Services.Implementations
{
    public class FirebaseService : IFirebaseService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;
        private readonly ILogger<FirebaseService> _logger;

        public FirebaseService(IConfiguration configuration, ILogger<FirebaseService> logger)
        {
            _logger = logger;

            try
            {
                // Get the Firebase section
                var firebaseSection = configuration.GetSection("Firebase");
                if (!firebaseSection.Exists())
                {
                    _logger.LogError("Firebase configuration section not found");
                    throw new ArgumentException("Firebase configuration section not found");
                }

                var credentialsPath = firebaseSection.GetValue<string>("Credentials");
                if (string.IsNullOrEmpty(credentialsPath))
                {
                    _logger.LogError("Firebase credentials path is missing in configuration");
                    throw new ArgumentException("Firebase credentials path is missing in configuration.");
                }

                credentialsPath = Path.Combine(AppContext.BaseDirectory, credentialsPath);
                _logger.LogInformation("Loading Firebase credentials from: {Path}", credentialsPath);

                if (!File.Exists(credentialsPath))
                {
                    _logger.LogError("Firebase credentials file not found at: {Path}", credentialsPath);
                    throw new FileNotFoundException($"Firebase credentials file not found at path: {credentialsPath}");
                }

                var credential = GoogleCredential.FromFile(credentialsPath);
                _logger.LogInformation("Successfully loaded Firebase credentials");

                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = credential
                    });
                    _logger.LogInformation("Created new Firebase App instance");
                }

                _storageClient = StorageClient.Create(credential);
                if (_storageClient == null)
                {
                    _logger.LogError("StorageClient is null. Ensure Firebase credentials are correct.");
                    throw new NullReferenceException("StorageClient is not initialized.");
                }

                _bucketName = configuration["Firebase:StorageBucket"];
                if (string.IsNullOrEmpty(_bucketName))
                {
                    _logger.LogError("Firebase storage bucket name is missing in configuration");
                    throw new ArgumentException("Firebase storage bucket name is missing in configuration");
                }

                _logger.LogInformation("Firebase service initialized successfully with bucket: {BucketName}", _bucketName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing Firebase service");
                throw;
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder, string numberId)
        {
            try
            {
                if (file == null)
                {
                    _logger.LogError("File is null in UploadFileAsync.");
                    throw new ArgumentNullException(nameof(file), "File cannot be null.");
                }

                // Generate unique filename
                var fileName = $"{folder}/avatar-{numberId}{Path.GetExtension(file.FileName)}";

                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                // Upload to Firebase Storage with public read access
                var dataObject = await _storageClient.UploadObjectAsync(
                    _bucketName,
                    fileName,
                    file.ContentType,
                    memoryStream,
                    new UploadObjectOptions { PredefinedAcl = PredefinedObjectAcl.PublicRead }
                );

                // Log upload success
                _logger.LogInformation("File uploaded successfully to: {FileName}", fileName);

                // Return public URL
                var publicUrl = $"https://storage.googleapis.com/{_bucketName}/{fileName}";
                _logger.LogInformation("Public URL generated: {PublicUrl}", publicUrl);
                return publicUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "File upload failed for file: {FileName}", file.FileName);
                throw new Exception("File upload failed", ex);
            }
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            try
            {
                _logger.LogInformation("Attempting to delete file: {FileUrl}", fileUrl);

                // Extract object name from URL
                var uri = new Uri(fileUrl);
                var objectName = uri.LocalPath.TrimStart('/');
                if (objectName.StartsWith(_bucketName))
                {
                    objectName = objectName.Substring(_bucketName.Length + 1);
                }

                _logger.LogInformation("Extracted object name: {ObjectName}", objectName);

                // Delete from Firebase Storage
                await _storageClient.DeleteObjectAsync(_bucketName, objectName);

                _logger.LogInformation("Successfully deleted file: {ObjectName}", objectName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "File deletion failed for URL: {FileUrl}", fileUrl);
                throw new Exception("File deletion failed", ex);
            }
        }

        public async Task<string> GetFileUrlAsync(string path)
        {
            try
            {
                _logger.LogInformation("Generating URL for path: {Path}", path);

                // Ensure path is valid
                if (string.IsNullOrEmpty(path))
                {
                    _logger.LogError("Path is null or empty in GetFileUrlAsync");
                    throw new ArgumentNullException(nameof(path), "Path cannot be null or empty.");
                }

                // Check if the file exists in Firebase Storage
                try
                {
                    var objectMetadata = await _storageClient.GetObjectAsync(_bucketName, path);
                    _logger.LogInformation("File found in Firebase Storage: {Path}", path);
                }
                catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogError("File not found in Firebase Storage: {Path}", path);
                    throw new FileNotFoundException($"File not found at path: {path}");
                }

                // Generate public URL (assuming the file is public)
                var publicUrl = $"https://storage.googleapis.com/{_bucketName}/{path}";
                _logger.LogInformation("Generated public URL: {PublicUrl}", publicUrl);
                return publicUrl;

                // Optional: Generate a signed URL with expiration (uncomment if needed)
                /*
                var signedUrl = await _storageClient.CreateSignedUrlAsync(
                    _bucketName,
                    path,
                    TimeSpan.FromHours(1), // URL valid for 1 hour
                    null,
                    HttpMethod.Get
                );
                _logger.LogInformation("Generated signed URL: {SignedUrl}", signedUrl);
                return signedUrl;
                */
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate URL for path: {Path}", path);
                throw new Exception("Failed to generate file URL", ex);
            }
        }
    }
}