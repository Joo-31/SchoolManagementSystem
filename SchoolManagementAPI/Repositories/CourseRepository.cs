using Microsoft.EntityFrameworkCore;
using SchoolManagementAPI.DataBase;
using SchoolManagementAPI.DTOs.Responses;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;

namespace SchoolManagementAPI.Repositories
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(SchoolDbContext context) : base(context) { }

        // ✅ Override GetById
        public new Course? GetById(int id)
        {
            return _dbSet
                .Include(c => c.Grade)
                .Include(c => c.Teacher)   // ← الجديد
                .FirstOrDefault(c => c.Id == id);
        }

        // ✅ Override GetAll
        public new IEnumerable<Course> GetAll()
        {
            return _dbSet
                .Include(c => c.Grade)
                .Include(c => c.Teacher)   // ← الجديد
                .ToList();
        }
        public IEnumerable<Course> GetCoursesByGradeId(int gradeId)
        {
            return _dbSet
                .Include(c => c.Grade).Include(c => c.Teacher)
                .Where(c => c.GradeId == gradeId)
                .ToList();
        }

        public IEnumerable<Course> GetCoursesOrderedByCredits()
        {
            return _dbSet
                .Include(c => c.Grade).Include(c => c.Teacher)
                .OrderByDescending(c => c.Credits)
                .ToList();
        }

        public IEnumerable<Course> GetCoursesWithCreditsMoreThan(int credits)
        {
            return _dbSet
                .Include(c => c.Grade).Include(c => c.Teacher)
                .Where(c => c.Credits > credits)
                .ToList();
        }

        public Dictionary<int, int> GetCourseCountByGrade()
        {
            return _dbSet
                .GroupBy(c => c.GradeId)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public double GetAverageCredits()
        {
            if (!_dbSet.Any())
                return 0;

            return _dbSet.Average(c => c.Credits);
        }

        public Course? GetCourseWithMaxCredits()
        {
            return _dbSet
                .Include(c => c.Grade)
                .Include(c => c.Teacher)
                .OrderByDescending(c => c.Credits)
                .FirstOrDefault();
        }

        public Course? GetCourseWithMinCredits()
        {
            return _dbSet
                .Include(c => c.Grade)
                .Include(c => c.Teacher)
                .OrderBy(c => c.Credits)
                .FirstOrDefault();
        }

        public new PagedResult<Course> GetPaged(int pageNumber, int pageSize)
        {
            var query = _dbSet
                .Include(c => c.Grade)
                .Include(c => c.Teacher)
                .AsNoTracking()
                .AsQueryable();

            var totalCount = query.Count();

            var data = query
                .OrderBy(c => c.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<Course>
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