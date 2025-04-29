using System.Linq.Expressions;

namespace Adis.Dal.Interfaces
{
    /// <summary>
    /// Реализует паттерн Спецификация
    /// </summary>
    /// <typeparam name="T">Тип данных, для которых нужно указать спецификацию</typeparam>
    public interface ISpecification<T>
    {
        /// <summary>
        /// Лямбда-выражение с условием для фильтрации
        /// </summary>
        Expression<Func<T, bool>>? Criteria { get; }

        /// <summary>
        /// Лямбда-выражения с подключением связанных таблиц
        /// </summary>
        List<Expression<Func<T, object>>> Includes { get; }

        /// <summary>
        /// Лямбда-выражение для сортировки по возрастанию
        /// </summary>
        Expression<Func<T, object>>? OrderBy { get; }

        /// <summary>
        /// Лямбда-выражение для сортировки по убыванию
        /// </summary>
        Expression<Func<T, object>>? OrderByDescending { get; }

        /// <summary>
        /// Размер списка
        /// </summary>
        int Take { get; }

        /// <summary>
        /// Количество пропускаемых элементов в списке
        /// </summary>
        int Skip { get; }

        /// <summary>
        /// Активна ли пагинация
        /// </summary>
        bool IsPagingEnabled { get; }
    }
}
