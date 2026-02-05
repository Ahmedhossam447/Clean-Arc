using CleanArc.Core.Entites;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Animal> AnimalRepository { get; }
        IRepository<Request> RequestRepository { get; }
        IRepository<T> Repository<T>() where T : class;
        Task<int> SaveChangesAsync(CancellationToken token =default);
        Task BeginTransactionAsync(CancellationToken token = default);
        Task CommitTransactionAsync(CancellationToken token = default);
        Task RollbackTransactionAsync(CancellationToken token = default);

    }
}
