using Adis.Dal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Adis.Dal.Specifications
{
    /// <summary>
    /// Позволяет использовать спецификации в EFCore
    /// </summary>
    public static class SpecificationEvaluator
    {
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
