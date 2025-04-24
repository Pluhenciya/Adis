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
        private IQueryable<T> ApplySpecification(ISpecification<T>? specification)
        {
            return SpecificationEvaluator.GetQuery(_dbContext.Set<T>().AsQueryable(), specification);
        }
    }
}
