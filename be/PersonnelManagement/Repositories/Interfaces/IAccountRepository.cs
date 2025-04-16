using PersonnelManagement.Models.DTOs;
using PersonnelManagement.Models.Entities;

namespace PersonnelManagement.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<List<AccountDtos>> GetAccounts();
        Task<Account?> GetAccountById(Guid accountId);
        Task<Account?> GetAccountByUsernameAsync(string username);
        Task<bool> UpdateAccount(Guid accountId, string newEmail, string newName, Guid newRoleAccountId);
        Task<bool> AddAccount(string email, string username, string password, Guid roleAccount);
        bool DeleteAccountById(Guid accountId);
        Account? Authenticate(string username, string password);
        Task SaveRefreshToken(Guid accountId, string refreshToken);
        Task<Token?> GetRefreshToken(Guid accountId);
        Task<Token?> GetRefreshTokenByToken(string token);
        Task RevokeRefreshToken(Token refreshToken);
        Task<bool> ForgotPassword(string email, string username);
        Task<bool> VerifyCode(string code, string username);
        Task<bool> ChangePass(string newPass, string username);
    }
}
