using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Specifications
{
    /// <summary>
    /// Спецификация для получения токена обновления по его значению и идентификатору пользователя, которому он принадлежит
    /// </summary>
    public class RefreshTokenSpecification : Specification<RefreshToken>
    {
        public RefreshTokenSpecification(string token, int idUser)
        {
            Criteria = rt => rt.Token == token && rt.IdUser == idUser;
            Includes.Add(rt => rt.User);
        }
    }
}
