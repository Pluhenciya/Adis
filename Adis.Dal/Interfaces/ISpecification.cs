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

        Expression<Func<T, object>>? OrderBy { get; }
        Expression<Func<T, object>>? OrderByDescending { get; }
        int Take { get; }
        int Skip { get; }
        bool IsPagingEnabled { get; }
    }
}
