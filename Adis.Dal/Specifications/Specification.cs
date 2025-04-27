using Adis.Dal.Interfaces;
using Adis.Dm;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Specifications
{
    /// <summary>
    /// Базовый класс спецификации
    /// </summary>
    /// <typeparam name="T">Тип, для которого нужна спецификация</typeparam>
    public abstract class Specification<T> : ISpecification<T>
    {
        protected Specification(Expression<Func<T, bool>>? criteria = null)
        {
            Criteria = criteria;
        }

        public Expression<Func<T, bool>>? Criteria { get; protected set; }
        public List<Expression<Func<T, object>>> Includes { get; } = new();
        public Expression<Func<T, object>>? OrderBy { get; private set; }
        public Expression<Func<T, object>>? OrderByDescending { get; private set; }
        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPagingEnabled { get; private set; }

        /// <summary>
        /// Добавляет связанную таблицу
        /// </summary>
        /// <param name="includeExpression">Лямбда-выражения с подключением связанной таблицы</param>
        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        /// <summary>
        /// Комбинирует фильтры
        /// </summary>
        /// <param name="newCriteria">Новый фильтр</param>
        protected void ApplyCriteria(Expression<Func<T, bool>> newCriteria)
        {
            Criteria = Criteria == null
                ? newCriteria
                : PredicateBuilder.And(Criteria, newCriteria);
        }

        protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
            OrderByDescending = null;
        }

        protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
            OrderBy = null;
        }

        // Методы для пагинации
        protected void ApplyPaging(int skip, int take)
        {
            if (take <= 0)
                throw new ArgumentException("Take must be greater than 0", nameof(take));

            if (skip < 0)
                throw new ArgumentException("Skip must be greater than or equal to 0", nameof(skip));

            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }

        protected void DisablePaging()
        {
            IsPagingEnabled = false;
            Skip = 0;
            Take = 0;
        }
    }
}
