using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MazenWebApp.DataAccess.Data;
using MazenWebApp.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MazenWebApp.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T :class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            dbSet = _context.Set<T>();
            //_context.Products.Include(p => p.Category);
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? includePropeties = null)
        {
            IQueryable<T> query = dbSet;

            if (!string.IsNullOrEmpty(includePropeties))
            {
                foreach (var includeProp in 
                    includePropeties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            query = query.Where(filter);

            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(string? includePropeties = null)
        {
            IQueryable<T> query = dbSet;

            if (!string.IsNullOrEmpty(includePropeties))
            {
                foreach (var includeProp in
                    includePropeties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return query.ToList();
        }

        public void Remove(T entity)
        {
            _context.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _context.RemoveRange(entities);
        }
    }
}
