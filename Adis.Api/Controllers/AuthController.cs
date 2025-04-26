using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Adis.Dm;
using Adis.Bll.Dtos.Auth;
using Adis.Bll.Interfaces;

namespace Adis.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request.Email, request.Password);

            return result.Success
                ? Ok(new AuthResponse { AccessToken = result.Token, ExpiresIn = result.ExpiresIn, TokenType = "Bearer"})
                : Unauthorized(new { Errors = result.Errors });
        }
    }

}