using ChatHub.DAL.Datas;
using ChatHub.DAL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.DAL.Repository.Implementation
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ChatHubDbContext _context;

        public Repository(ChatHubDbContext context)
        {
            _context = context;
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<T>> WhereAsync(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().Where(expression).ToListAsync();
        }

        public async Task<IEnumerable<T>> OrderByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().OrderBy(predicate).ToListAsync();
        }
        public async Task<IEnumerable<T>> OrderByDescAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, int?>> orderByDesc)
        {
            return await _context.Set<T>().Where(predicate).OrderByDescending(orderByDesc).ToListAsync();
        }
        public async Task<List<T>> OrderBy(Expression<Func<T, bool>> predicate, Expression<Func<T, DateTime?>> orderBy)
        {
            return await _context.Set<T>().Where(predicate).OrderBy(orderBy).ToListAsync();
        }

        public async Task<bool> AnyAsync()
        {
            return await _context.Set<T>().AnyAsync();
        }
        public async Task<bool> CheckIdIfExists(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AnyAsync(predicate);
        }

        public bool CheckTokenIfExist(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Any(predicate);
        }

        public T AddToken(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
            return entity;
        }

    }
}
