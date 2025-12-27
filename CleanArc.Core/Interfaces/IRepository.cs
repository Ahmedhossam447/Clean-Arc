using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Core.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> GetAsync(Func<TEntity, bool> predicate);
        Task<TEntity> GetByIdAsync(int id);
        Task<TEntity> AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(int id);
        int SaveChanges();
        Task SaveChangesAsync();
    }
}
