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

        /// <summary>
        /// Добавляет сортировку по возрастанию
        /// </summary>
        /// <param name="orderByExpression">Лямбда-выражения сортировки</param>
        protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
            OrderByDescending = null;
        }

        /// <summary>
        /// Добавляет сортировку по убываннию
        /// </summary>
        /// <param name="orderByDescendingExpression">Лямбда-выражения сортировки</param>
        protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
            OrderBy = null;
        }

        /// <summary>
        /// Добавляет пагинацию
        /// </summary>
        /// <param name="skip">Количество пропускаемых элементов</param>
        /// <param name="take">Количество возвращаемых элементов</param>
        /// <exception cref="ArgumentException">Выбрасывается, если один или оба параметра меньше нуля, также если количество возвращаемых элементов равно 0</exception>
        protected void ApplyPaging(int skip, int take)
        {
            if (take <= 0)
                throw new ArgumentException("Количество возвращаемых элементов должно быть больше 0", nameof(take));

            if (skip < 0)
                throw new ArgumentException("Количество пропускаемых элементов должно быть больше или равно 0", nameof(skip));

            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }

        /// <summary>
        /// Выключает пагинацию
        /// </summary>
        protected void DisablePaging()
        {
            IsPagingEnabled = false;
            Skip = 0;
            Take = 0;
        }
    }
}
