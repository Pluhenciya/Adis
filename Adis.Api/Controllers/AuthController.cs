using Adis.Bll.Dtos.Auth;
using Adis.Bll.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Adis.Api.Controllers
{
    /// <summary>
    /// Позволяет авторизовываться пользавателям
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        /// <summary>
        /// Возврашает два токена для Bearer авторизации по данным для входа
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     POST /api/auth/login
        ///     {
        ///         "email": "ivan.petrov@example.com",
        ///         "password": "Ispp1234_"
        ///     }
        ///
        /// </remarks>
        /// <param name="request">Данные для авторизации(почта и пароль)</param>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="401">Ошибка авторизации</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request.Email, request.Password);

            return result.Success
                ? Ok(new AuthResponse { AccessToken = result.Token, ExpiresIn = result.ExpiresIn, TokenType = "Bearer", RefreshToken = result.RefreshToken })
                : Unauthorized(new { result.Errors });
        }

        /// <summary>
        /// Возврашает два токена для Bearer авторизации по токену обновления
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     POST /api/auth/login
        ///     {
        ///         "accessToken": "access_token",
        ///         "refreshToken": "refresh_token"
        ///     }
        ///
        /// </remarks>
        /// <param name="request">Токены для обновления</param>
        /// <response code="200">Успешное выполнение</response>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await _authService.RefreshTokenAsync(request.AccessToken, request.RefreshToken);

            return result.Success
                ? Ok(new AuthResponse
                {
                    AccessToken = result.Token,
                    ExpiresIn = result.ExpiresIn,
                    TokenType = "Bearer",
                    RefreshToken = result.RefreshToken
                })
                : BadRequest(new { result.Errors });
        }
    }
}