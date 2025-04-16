using PersonnelManagement.Models.DTOs;

namespace PersonnelManagement.Services.Interfaces
{
    public interface IAuthService
    {
        Task<TokenRes?> LoginAsync(LoginAuthReq loginReq);
    }
}
