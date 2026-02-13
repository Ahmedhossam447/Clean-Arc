using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections;

namespace CleanArc.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork, IAsyncDisposable
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;
        private IRepository<Animal>? _animalRepository;
        private Dictionary<string, object>? _repositories;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IRepository<Animal> AnimalRepository
        {
            get
            {
                _animalRepository ??= new Repository<Animal>(_context);
                return _animalRepository;
            }
        }


        public IRepository<T> Repository<T>() where T : class
        {
            _repositories ??= new Dictionary<string, object>();

            var type = typeof(T).Name;
            if (!_repositories.ContainsKey(type))
            {
                var repositoryInstance = new Repository<T>(_context);
                _repositories.Add(type, repositoryInstance);
            }
            return (IRepository<T>)_repositories[type];
        }

        public async Task BeginTransactionAsync(CancellationToken token = default)
        {
            if (_transaction != null)
                return;

            _transaction = await _context.Database.BeginTransactionAsync(token);
        }

        public async Task CommitTransactionAsync(CancellationToken token = default)
        {
            try
            {
                await _context.SaveChangesAsync(token);
                if (_transaction != null)
                {
                    await _transaction.CommitAsync(token);
                }
            }
            catch
            {
                await RollbackTransactionAsync(token);
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken token = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(token);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken token = default)
        {
            return await _context.SaveChangesAsync(token);
        }

        public async Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
        {
            return await _context.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        // Sync dispose — safety net for any leftover transaction
        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
                _transaction = null;
            }
            _context.Dispose();
        }

        // Async dispose — preferred path, registered automatically by DI
        public async ValueTask DisposeAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
            await _context.DisposeAsync();
        }
    }
}
