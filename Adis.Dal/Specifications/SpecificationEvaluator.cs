using Adis.Dal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Adis.Dal.Specifications
{
    /// <summary>
    /// Позволяет использовать спецификации в EFCore
    /// </summary>
    public static class SpecificationEvaluator
    {
        /// <summary>
        /// Возвращает запрос на основе спецификации
        /// </summary>
        /// <typeparam name="T">Тип объекта спецификации</typeparam>
        /// <param name="inputQuery">Запрос без спецификации</param>
        /// <param name="specification">Спецификация</param>
        /// <returns>Запрос со спецификацией</returns>
        public static IQueryable<T> GetQuery<T>(
            IQueryable<T> inputQuery,
            ISpecification<T>? specification)
            where T : class
        {
            if (specification == null)
                return inputQuery;

            var query = inputQuery;

            if (specification.Criteria != null)
                query = query.Where(specification.Criteria);

            query = specification.Includes
                .Aggregate(query, (current, include) => current.Include(include));

            return query;
        }
    }
}
