using Adis.Dal.Data;
using Adis.Dal.Interfaces;
using Adis.Dal.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Adis.Dal.Repositories
{
    /// <summary>
    /// Реализует стандартные методы репозитория через EFCore
    /// </summary>
    /// <typeparam name="T">Тип объекта, с которым работает репозиторий</typeparam>
    public class EFGenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _dbContext;

        public EFGenericRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> AddAsync(T entity)
        {
            var entry = await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entry.Entity;
        }

        /// <inheritdoc/>
        /// <exception cref="KeyNotFoundException">Возникает когда объект с таким идентификаторм не существует</exception>
        public async Task DeleteAsync(int id)
        {
            var entity = await _dbContext.FindAsync<T>(id)
                ?? throw new KeyNotFoundException($"{typeof(T).Name} с ID {id} не найден");

            _dbContext.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAsync(ISpecification<T>? specification = null)
        {
            return await ApplySpecification(specification).ToListAsync();
        }

        /// <inheritdoc/>
        /// <exception cref="KeyNotFoundException">Возникает когда объект с таким идентификаторм не существует</exception>
        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.FindAsync<T>(id)
                ?? throw new KeyNotFoundException($"{typeof(T).Name} с ID {id} не найден");
        }

        public async Task<T> UpdateAsync(T entity)
        {
            var entry = _dbContext.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entry.Entity;
        }
        public IQueryable<T> ApplySpecification(ISpecification<T>? spec)
        {
            var query = _dbContext.Set<T>().AsQueryable();

            if (spec == null) return query;

            foreach (var include in spec.IncludeStrings)
            {
                query = query.Include(include);
            }

            foreach (var include in spec.Includes)
            {
                query = query.Include(include);
            }

            // Фильтрация
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            // Сортировка
            if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }
            else if (spec.OrderByDescending != null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }

            // Пагинация
            if (spec.IsPagingEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            return query;
        }
    }
}
