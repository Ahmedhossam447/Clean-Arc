using CleanArc.Core.Interfaces;
using CleanArc.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Infrastructure.Persistence
{
    internal class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<TEntity> _dbSet;
        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();

        }
        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }
        public async void Delete(int id)
        {
            var Data =await _dbSet.FindAsync(id);
            if (Data != null)
            {
                _dbSet.Remove(Data);
            }
        }
        public int SaveChanges()
        {
          return _context.SaveChanges();
        }
        public async Task<IEnumerable<TEntity>> GetAsync(Func<TEntity, bool> predicate)
        {
            return await Task.Run(() => _dbSet.AsQueryable().Where(predicate));
        }
        public async Task SaveChangesAsync()
        {
            await  _context.SaveChangesAsync();
        }
    }
}
