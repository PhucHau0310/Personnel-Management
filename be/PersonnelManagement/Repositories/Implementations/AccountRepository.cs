using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersonnelManagement.Data;
using PersonnelManagement.Models.DTOs;
using PersonnelManagement.Models.Entities;
using PersonnelManagement.Repositories.Interfaces;
using PersonnelManagement.Services.Interfaces;

namespace PersonnelManagement.Repositories.Implementations
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public AccountRepository(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<List<AccountDtos>> GetAccounts()
        {
            try
            {
                return await _context.Accounts
                    .AsNoTracking()
                    .Select(a => new AccountDtos
                    {
                        Id = a.Id,
                        Username = a.Username,
                        RoleAccountId = a.RoleAccountId,
                        CreatedAt = a.CreatedAt,
                        UpdatedAt = a.UpdatedAt
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting accounts: {ex.Message}");
                return new List<AccountDtos>();
            }
        }

        public async Task<Account?> GetAccountById(Guid accountId)
        {
            var user = await _context.Accounts
                         .Include(u => u.RefreshTokens)
                         .Include(u => u.RoleAccount)
                         .FirstOrDefaultAsync(u => u.Id == accountId);
            return user;
        }

        public async Task<Account?> GetAccountByUsernameAsync(string username)
        {
            var user = await _context.Accounts
                         .Include(u => u.RefreshTokens)
                         .Include(u => u.RoleAccount)
                         .FirstOrDefaultAsync(u => u.Username == username);

            return user;
        }

        public bool DeleteAccountById(Guid accountId)
        {
            var user = _context.Accounts.FirstOrDefault(u => u.Id == accountId);
            if (user == null)
            {
                return false;
            }

            _context.Accounts.Remove(user);
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> UpdateAccount(Guid accountId, string newEmail, string newName, Guid newRoleAccountId)
        {
            if (Guid.Empty == accountId)
            {
                return false;
            }

            var accountFound = await _context.Accounts.FirstOrDefaultAsync(u => u.Id == accountId);
            if (accountFound == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(newName))
            {
                accountFound.Username = newName;
            }

            if (!string.IsNullOrEmpty(newEmail))
            {
                accountFound.Email = newEmail;
            }

            if (newRoleAccountId != Guid.Empty)
            {
                accountFound.RoleAccountId = newRoleAccountId;
            }

            accountFound.UpdatedAt = DateTime.UtcNow.AddHours(7);

            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> AddAccount(string email, string username, string password, Guid roleAccountId)
        {
            var existingUser = await _context.Accounts.FirstOrDefaultAsync(u => u.Username == username);
            if (existingUser != null) return false;

            var newAccount = new Account
            {
                Email = "",
                Username = username,
                RoleAccountId = roleAccountId,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                CreatedAt = DateTime.UtcNow.AddHours(7),
                UpdatedAt = DateTime.UtcNow.AddHours(7),
            };

            _context.Accounts.Add(newAccount);
            await _context.SaveChangesAsync();

            return true;
        }

        public Account? Authenticate(string username, string password)
        {
            var user = _context.Accounts
                .Include(u => u.RoleAccount)
                .FirstOrDefault(u => u.Username == username);

            if (user == null) return null;

            if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user;
            }

            return null;
        }

        public async Task SaveRefreshToken(Guid accountId, string refreshToken)
        {
            var account = await _context.Accounts.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Id == accountId);
            if (account == null) return;

            // Mark old refresh tokens as revoked
            foreach (var token in account.RefreshTokens)
            {
                token.IsRevoked = true;
            }

            account.RefreshTokens.Add(new Token
            {
                AccountId = accountId,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            });

            await _context.SaveChangesAsync();
        }

        public async Task<Token?> GetRefreshToken(Guid accountId)
        {
            var token = await _context.Tokens
                .Where(t => t.AccountId == accountId && t.Expiration > DateTime.UtcNow.AddHours(7) && !t.IsRevoked)
                .OrderByDescending(t => t.Expiration)
                .FirstOrDefaultAsync();

            return token ?? null;
        }

        public async Task<Token?> GetRefreshTokenByToken(string token)
        {
            var tokenRes = await _context.Tokens
                    .Where(t => t.RefreshToken == token && t.Expiration > DateTime.UtcNow.AddHours(7) && !t.IsRevoked)
                    .OrderByDescending(t => t.Expiration)
                    .FirstOrDefaultAsync();

            return tokenRes ?? null;
        }

        public async Task RevokeRefreshToken(Token refreshToken)
        {
            refreshToken.IsRevoked = true;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ForgotPassword(string email, string username)
        {
            var user = await _context.Accounts.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null) return false;

            var code = BCrypt.Net.BCrypt.HashPassword(username);

            var emailBody = $@"
            <h2>Change password</h2>
            <p>Hi, {user.Username}</p>
            <p>Here is the code, enter it on the verification page to proceed to the next step: <b>{code.ToString()}</b></p>";

            await _emailService.SendEmailAsync(email, "Change password", emailBody);
            return true;
        }

        public async Task<bool> VerifyCode(string code, string username)
        {
            bool isCodeValid = BCrypt.Net.BCrypt.Verify(username, code);
            var user = await _context.Accounts.SingleOrDefaultAsync(u => u.Username == username && isCodeValid);

            if (user == null) return false;
            return true;
        }

        public async Task<bool> ChangePass(string newPass, string username)
        {
            var user = await _context.Accounts.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null) return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPass);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
