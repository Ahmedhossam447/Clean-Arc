using CleanArc.Core.Entities;

namespace CleanArc.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Animal> AnimalRepository { get; }
        IRepository<T> Repository<T>() where T : class;
        Task<int> SaveChangesAsync(CancellationToken token =default);
        Task BeginTransactionAsync(CancellationToken token = default);
        Task CommitTransactionAsync(CancellationToken token = default);
        Task RollbackTransactionAsync(CancellationToken token = default);
        Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters);

    }
}
