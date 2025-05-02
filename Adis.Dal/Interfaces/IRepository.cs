using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Interfaces
{
    /// <summary>
    /// Содержит описание стандартных методов репозитория
    /// </summary>
    /// <typeparam name="T">Тип объекта, с которым работает репозиторий</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Возвращает все объекты данного типа
        /// </summary>
        /// <param name="specification">Фильтры и подключение связанных таблиц</param>
        /// <returns>Список объектов данного типа</returns>
        Task<IEnumerable<T>> GetAsync(ISpecification<T>? specification = null);

        /// <summary>
        /// Возвращает объект данного типа по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <returns>Запрашиваемый объект данного типа</returns>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Изменяет объект данного типа
        /// </summary>
        /// <param name="entity">Новые данные объекта данного типа</param>
        /// <returns>Изменненый объект с новыми данными</returns>
        public Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Добавляет объект данного типа
        /// </summary>
        /// <param name="entity">Данные нового объекта данного типа</param>
        /// <returns>Созданный объект</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Удаляет объект данного типа по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        Task DeleteAsync(int id);
    }
}
