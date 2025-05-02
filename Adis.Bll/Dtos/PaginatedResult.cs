using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Dtos
{
    /// <summary>
    /// Список элементов класса со свойствами для пагинации
    /// </summary>
    /// <typeparam name="T">Тип объектов списка</typeparam>
    public class PaginatedResult<T>
    {
        /// <summary>
        /// Список элементов
        /// </summary>
        public IEnumerable<T> Items { get; set; } = null!;

        /// <summary>
        /// Общее количество элементов
        /// </summary>
        public int TotalCount { get; set; }
    }
}
