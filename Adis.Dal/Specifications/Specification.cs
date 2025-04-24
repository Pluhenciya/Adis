using Adis.Dal.Interfaces;
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
    }
}
