using Microsoft.EntityFrameworkCore;
using SchoolManagementAPI.DataBase;
using SchoolManagementAPI.DTOs.Responses;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;

namespace SchoolManagementAPI.Repositories
{
    public class MarkRepository : Repository<Mark>, IMarkRepository
    {
        public MarkRepository(SchoolDbContext context) : base(context) { }

        // ✅ Override GetById عشان يعمل Include
        public new Mark? GetById(int id)
        {
            return _dbSet
                .Include(m => m.Student)
                .Include(m => m.Course)
                .Include(m => m.Teacher)
                .FirstOrDefault(m => m.Id == id);
        }

        // ✅ Override GetAll عشان يعمل Include
        public new IEnumerable<Mark> GetAll()
        {
            return _dbSet
                .Include(m => m.Student)
                .Include(m => m.Course)
                .Include(m => m.Teacher)
                .ToList();
        }

        public IEnumerable<Mark> GetByStudentId(int studentId)
        {
            return _dbSet
                .Include(m => m.Course)
                .Include(m => m.Teacher)
                .Where(m => m.StudentId == studentId)
                .ToList();
        }

        public IEnumerable<Mark> GetByCourseId(int courseId)
        {
            return _dbSet
                .Include(m => m.Student)
                .Include(m => m.Teacher)
                .Where(m => m.CourseId == courseId)
                .ToList();
        }

        public IEnumerable<Mark> GetByTeacherId(int teacherId)
        {
            return _dbSet
                .Include(m => m.Student)
                .Include(m => m.Course)
                .Where(m => m.TeacherId == teacherId)
                .ToList();
        }


        public PagedResult<Mark> GetPaged(int pageNumber, int pageSize, int? teacherId = null)
        {
            var query = _dbSet
                .Include(m => m.Student)
                .Include(m => m.Course)
                .Include(m => m.Teacher)
                .AsNoTracking()
                .AsQueryable();

            if (teacherId.HasValue)
            {
                query = query.Where(m => m.TeacherId == teacherId.Value);
            }

            var totalCount = query.Count();

            var data = query
                .OrderByDescending(m => m.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<Mark>
            {
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }
    }
}
    