using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Interfaces
{
    /// <summary>
    /// Позволяет управлять токенами обновления
    /// </summary>
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        /// <summary>
        /// Возвращает токен по идентификатору пользователя и токену обновления
        /// </summary>
        /// <param name="token">Токен обновления</param>
        /// <param name="idUser">Идентификатор пользователя, которому принадлежит токен</param>
        /// <returns>Полные данные токена обновления</returns>
        public Task<RefreshToken?> GetRefreshTokenByIdUserWithTokenAsync(string token, int idUser);
    }
}
