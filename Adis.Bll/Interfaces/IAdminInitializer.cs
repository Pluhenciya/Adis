using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Interfaces
{
    /// <summary>
    /// Позволяет создавать стартого администратора
    /// </summary>
    public interface IAdminInitializer
    {
        /// <summary>
        /// Cоздает стартого администратора
        /// </summary>
        public Task InitializeAsync();
    }
}
