using System.Linq.Expressions;
using SchoolManagementAPI.DTOs.Responses;


namespace SchoolManagementAPI.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        // ✅ Read
        T? GetById(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);

        // ✅ Pagination
        PagedResult<T> GetPaged(int pageNumber, int pageSize);

        // ✅ Write
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);

        // ✅ Save
        void SaveChanges();
    }
}