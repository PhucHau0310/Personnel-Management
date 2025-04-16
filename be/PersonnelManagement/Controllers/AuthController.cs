using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonnelManagement.Models.DTOs;
using PersonnelManagement.Services.Interfaces;

namespace PersonnelManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ITokenService tokenService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _tokenService = tokenService;
            _logger = logger;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginAuthReq request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isAuth = await _authService.LoginAsync(request);
            if (isAuth == null)
            {
                return Unauthorized(new
                {
                    Message = "Invalid username or password"
                });
            }

            return Ok(new
            {
                Message = "Login successful",
                Data = new
                {
                    AccessToken = isAuth.AccessToken,
                    RefreshToken = isAuth.RefreshToken
                }
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] TokenRes request)
        {
            var tokenResponse = await _tokenService.RefreshTokenAsync(request.RefreshToken);
            if (tokenResponse == null)
            {
                return Unauthorized(new
                {
                    Success = false,
                    Message = "Invalid refresh token."
                });
            }

            return Ok(new
            {
                Message = "Token refreshed",
                Data = tokenResponse
            });
        }
    }
}
