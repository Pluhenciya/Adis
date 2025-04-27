using Adis.Bll.Configurations;
using Adis.Bll.Dtos.Auth;
using Adis.Bll.Interfaces;
using Adis.Dal.Interfaces;
using Adis.Dm;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Services
{
    /// <inheritdoc cref="IAuthService"/>
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IOptions<JwtSettings> jwtSettings,
            IRefreshTokenRepository refreshTokenRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
            _refreshTokenRepository = refreshTokenRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return ErrorResult("Неверные данные");

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!signInResult.Succeeded)
                return ErrorResult("Неверные данные");

            if (string.IsNullOrEmpty(user.Email))
                return ErrorResult("Почта не указана");

            var token = await GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken(user);
            await SaveRefreshTokenAsync(refreshToken);

            return new AuthResult
            {
                Success = true,
                Token = token.Token,
                ExpiresIn = token.ExpiresIn,
                RefreshToken = refreshToken.Token
            };
        }

        private async Task<(string Token, int ExpiresIn)> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                signingCredentials: creds
            );

            return (
                new JwtSecurityTokenHandler().WriteToken(token),
                (int)_jwtSettings.TokenLifetime.TotalSeconds
            );
        }

        private AuthResult ErrorResult(params string[] errors)
        {
            return new AuthResult
            {
                Success = false,
                Errors = errors
            };
        }
        public async Task<AuthResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var principal = GetPrincipalFromExpiredToken(token);
            var idUser = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(idUser!);

            if (user == null)
                return ErrorResult("Invalid token");

            var storedRefreshToken = await _refreshTokenRepository.GetRefreshTokenByIdUserWithTokenAsync(refreshToken, user.Id);

            if (storedRefreshToken == null || storedRefreshToken.Expires < DateTime.UtcNow || storedRefreshToken.Revoked != null)
                return ErrorResult("Invalid refresh token");

            // Обновляем токен через репозиторий
            storedRefreshToken.Revoked = DateTime.UtcNow;
            storedRefreshToken.RevokedByIp = GetIpAddress();
            await _refreshTokenRepository.UpdateAsync(storedRefreshToken);

            // Генерируем новые токены
            var newAccessToken = await GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken(user);
            await _refreshTokenRepository.AddAsync(newRefreshToken);

            return new AuthResult
            {
                Success = true,
                Token = newAccessToken.Token,
                ExpiresIn = newAccessToken.ExpiresIn,
                RefreshToken = newRefreshToken.Token
            };
        }

        private RefreshToken GenerateRefreshToken(User user)
        {
            return new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = GetIpAddress(),
                IdUser = user.Id
            };
        }

        private async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _refreshTokenRepository.AddAsync(refreshToken);
        }

        private string GetIpAddress()
        {
            return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return principal;
        }
    }
}
