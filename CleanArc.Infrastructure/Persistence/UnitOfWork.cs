using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using CleanArc.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _transaction;
        private IRepository<Request> _requestRepository;
        private IRepository<Animal> _animalRepository;
        private Hashtable _repositories;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }


        public IRepository<Animal> AnimalRepository
        { 
            get
            {
                if (_animalRepository == null)
                {
                    _animalRepository = new Repository<Animal>(_context);
                }
                return _animalRepository;
            }
        }

        public IRepository<Request> RequestRepository
        { 
            get
            {
                if (_requestRepository == null)
                {
                    _requestRepository = new Repository<Request>(_context);
                }
                return _requestRepository;
            }
        }
        public IRepository<T> Repository<T>() where T : class
        {
          if(_repositories == null)
          {
            _repositories = new Hashtable();
          }
            var type = typeof(T).Name;
            if(!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);
                _repositories.Add(type, repositoryInstance);
            }
            return (IRepository<T>)_repositories[type];
        }

        public async Task BeginTransactionAsync(CancellationToken token = default)
        {
            if (_transaction != null)
            {
                return;
            }
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

        public  void Dispose()
        {
            _context.Dispose();
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
    }
}
