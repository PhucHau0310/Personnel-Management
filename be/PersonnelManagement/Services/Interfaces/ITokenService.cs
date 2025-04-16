using PersonnelManagement.Models.DTOs;
using System.Security.Claims;

namespace PersonnelManagement.Services.Interfaces
{
    public interface ITokenService
    {
        Task<TokenRes?> RefreshTokenAsync(string token);
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token);
    }
}
