using PersonnelManagement.Models.DTOs;
using PersonnelManagement.Repositories.Interfaces;
using PersonnelManagement.Services.Interfaces;
using System.Security.Claims;

namespace PersonnelManagement.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAccountRepository accountRepository, ITokenService tokenService, ILogger<AuthService> logger)
        {
            _accountRepository = accountRepository;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<TokenRes?> LoginAsync(LoginAuthReq loginReq)
        {
            if (string.IsNullOrEmpty(loginReq.Username))
            {
                _logger.LogError("Username is required");
                return null;
            }

            if (string.IsNullOrEmpty(loginReq.Password))
            {
                _logger.LogError("Password is required");
                return null;
            }

            var account = _accountRepository.Authenticate(loginReq.Username, loginReq.Password);
            if (account == null) return null;

            // Check if user already has a valid RefreshToken
            var existingToken = await _accountRepository.GetRefreshToken(account.Id);

            string refreshToken;
            if (existingToken != null)
            {
                // Use the existing RefreshToken if it's still valid
                refreshToken = existingToken.RefreshToken;
            }
            else
            {
                // if user doesn't have a valid RefreshToken, generate a new one
                refreshToken = _tokenService.GenerateRefreshToken();
                await _accountRepository.SaveRefreshToken(account.Id, refreshToken);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.Username),
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
            };

            //_logger.LogInformation($"Account Role: {account.RoleAuthorization.ToString()}");

            //if (account.RoleAuthorization != null)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, account.RoleAuthorization.ToString()));
            //}

            var accessToken = _tokenService.GenerateAccessToken(claims);

            return new TokenRes(accessToken, refreshToken);
        }
    }
}
