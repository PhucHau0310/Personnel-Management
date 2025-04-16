using Microsoft.IdentityModel.Tokens;
using PersonnelManagement.Models.DTOs;
using PersonnelManagement.Repositories.Interfaces;
using PersonnelManagement.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PersonnelManagement.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<TokenService> _logger;
        public TokenService(IConfiguration configuration, IAccountRepository accountRepository, ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _accountRepository = accountRepository;
            _logger = logger;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = _configuration["Jwt:ExpirationMinutes"]; // 60 minutes

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Int32.Parse(expiration)),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<TokenRes?> RefreshTokenAsync(string token)
        {
            var refreshToken = await _accountRepository.GetRefreshTokenByToken(token);
            if (refreshToken == null || refreshToken.Expiration < DateTime.UtcNow.AddHours(7) || refreshToken.IsRevoked)
            {
                return null;
            }

            await _accountRepository.RevokeRefreshToken(refreshToken);

            var account = await _accountRepository.GetAccountById(refreshToken.AccountId);

            if (account == null)
            {
                return null;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.Username),
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
            };


            //if (account.RoleAuthorization != null)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, account.RoleAuthorization.ToString()));
            //}

            var newAccessToken = GenerateAccessToken(claims);
            var newRefreshToken = GenerateRefreshToken();

            // save new token
            await _accountRepository.SaveRefreshToken(account.Id, newRefreshToken);

            return new TokenRes(newAccessToken, newRefreshToken);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            _logger.LogInformation($"Validating token: {token}");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                _logger.LogInformation($"Token validated successfully: {principal?.Identity?.Name}");
                _logger.LogInformation($"Token validated successfully: {principal?.Identity?.IsAuthenticated}");

                return principal ?? null;
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogError($"Token validation failed: {ex.Message}");
                return null;
            }
        }
    }
}
