using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CleanArc.Core.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        // Read operations - cancellable
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        
        // Write operations - NOT cancellable (must complete)
        Task<TEntity> AddAsync(TEntity entity);
        void Update(TEntity entity);
        Task Delete(int id);
        int SaveChanges();
        Task SaveChangesAsync();
    }
}
