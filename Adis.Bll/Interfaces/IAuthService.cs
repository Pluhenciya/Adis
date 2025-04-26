using Adis.Bll.Dtos.Auth;

namespace Adis.Bll.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(string email, string password);
    }
}
