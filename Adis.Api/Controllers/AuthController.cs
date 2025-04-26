using Adis.Bll.Dtos.Auth;
using Adis.Dm;
using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Adis.Api.Controllers
{
    /// <summary>
    /// Позволяет авторизоваться в API пользователям
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITokenService tokenService) : ControllerBase
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly ITokenService _tokenService = tokenService;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Invalid credentials");

            // Создаем IdentityServer контекст
            var context = new TokenCreationRequest
            {
                Subject = new IdentityServerUser(user.Id.ToString())
                {
                    AdditionalClaims = new[]
                    {
                new Claim(JwtClaimTypes.Email, user.Email),
                new Claim(JwtClaimTypes.Name, user.FullName ?? string.Empty),
                new Claim(JwtClaimTypes.Role, string.Join(",", await _userManager.GetRolesAsync(user)))
            }
                }.CreatePrincipal(),

                ValidatedRequest = new ValidatedRequest
                {
                    Client = new Client
                    {
                        ClientId = "website",
                        AllowedGrantTypes = GrantTypes.ResourceOwnerPassword
                    }
                }
            };

            // Создаем токен
            var token = await _tokenService.CreateAccessTokenAsync(context);
            var accessToken = await _tokenService.CreateSecurityTokenAsync(token);

            return Ok(new AuthResponse
            {
                AccessToken = accessToken,
                ExpiresIn = 3600,
                TokenType = "Bearer"
            });
        }
    }
}
