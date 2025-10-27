﻿using TicketWave.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TicketWave.Models;

namespace TicketWave.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private ApplicationdbContext _context;// = new();
        private DbSet<T> _dbSet;

        public Repository(ApplicationdbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // CRUD

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            var result = await _dbSet.AddAsync(entity, cancellationToken);
            return result.Entity;
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(T entity )
        {
            _dbSet.Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? includes = null,
            bool tracked = true,
            CancellationToken cancellationToken = default)
        {
            var entities = _dbSet.AsQueryable();

            if (expression is not null)
                entities = entities.Where(expression);

            if (includes is not null)
            {
                foreach (var item in includes)
                    entities = entities.Include(item);
            }

            if (!tracked)
                entities = entities.AsNoTracking();

            //entities = entities.Where(e => e.Status);

            return await entities.ToListAsync(cancellationToken);
        }

        public async Task<T?> GetOneAsync(
            Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? includes = null,
            bool tracked = true,
            CancellationToken cancellationToken = default)
        {
            return (await GetAsync(expression, includes, tracked, cancellationToken)).FirstOrDefault();
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}

