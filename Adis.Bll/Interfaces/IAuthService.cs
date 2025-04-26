using Adis.Bll.Dtos.Auth;

namespace Adis.Bll.Interfaces
{
    public interface IAuthService
    {
        public Task<AuthResult> LoginAsync(string email, string password);

        public Task<AuthResult> RefreshTokenAsync(string token, string refreshToken);
    }
}
