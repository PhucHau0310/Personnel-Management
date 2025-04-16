namespace PersonnelManagement.Services.Interfaces
{
    public interface IFirebaseService
    {
        Task<string> UploadFileAsync(IFormFile file, string folder, string numberId);
        Task DeleteFileAsync(string fileUrl);
        Task<string> GetFileUrlAsync(string filePath);
    }
}
