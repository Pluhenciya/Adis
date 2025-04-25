using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Helpers
{
    /// <summary>
    /// Позволяет хешировать данные
    /// </summary>
    public static class HashHelper
    {
        /// <summary>
        /// Хеширует данные с помощью SHA256
        /// </summary>
        /// <param name="rawData">Данные для хеширования</param>
        /// <returns>Захэшированные данные</returns>
        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
