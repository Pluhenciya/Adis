using Adis.Bll.Dtos.Auth;

namespace Adis.Bll.Interfaces
{
    /// <summary>
    /// Позволяет авторизовывать пользователей и выдавать токены
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Возвращает токены для авторизациии по данным пользователя
        /// </summary>
        /// <param name="email">Почта пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <returns>Ответ с токенами</returns>
        public Task<AuthResult> LoginAsync(string email, string password);

        /// <summary>
        /// Возвращает новые токены для авторизациии по токену обновления
        /// </summary>
        /// <param name="token">Токен доступа</param>
        /// <param name="refreshToken">Токен обновления</param>
        /// <returns>Ответ с новыми токенами</returns>
        public Task<AuthResult> RefreshTokenAsync(string token, string refreshToken);
    }
}
