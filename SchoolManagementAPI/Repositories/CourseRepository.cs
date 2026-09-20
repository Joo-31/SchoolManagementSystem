
using SchoolManagementAPI.DataBase;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;

namespace SchoolManagementAPI.Repositories
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(SchoolDbContext context) : base(context)
        {
        }

        // ✅ Methods خاصة بالـ Course
        public IEnumerable<Course> GetCoursesByGradeId(int gradeId)
        {
            return _dbSet.Where(c => c.GradeId == gradeId).ToList();
        }

        public IEnumerable<Course> GetCoursesOrderedByCredits()
        {
            return _dbSet
                .OrderByDescending(c => c.Credits)
                .ToList();
        }

        public IEnumerable<Course> GetCoursesWithCreditsMoreThan(int credits)
        {
            return _dbSet
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
                .OrderByDescending(c => c.Credits)
                .FirstOrDefault();
        }

        public Course? GetCourseWithMinCredits()
        {
            return _dbSet
                .OrderBy(c => c.Credits)
                .FirstOrDefault();
        }
    }
}