using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SchoolManagementAPI.DataBase;
using SchoolManagementAPI.Repositories.Interfaces;
using SchoolManagementAPI.DTOs.Responses;

namespace SchoolManagementAPI.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly SchoolDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(SchoolDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // ✅ Read
        public T? GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToList();
        }
        // ✅ Pagination
        public PagedResult<T> GetPaged(int pageNumber, int pageSize)
        {
            var totalCount = _dbSet.Count();

            var data = _dbSet
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<T>
            {
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        // ✅ Write
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        // ✅ Save
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}