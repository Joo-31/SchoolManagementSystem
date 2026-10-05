using SchoolManagementAPI.DataBase;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;

namespace SchoolManagementAPI.Repositories
{
    public class GradeRepository : Repository<Grade>, IGradeRepository
    {
        public GradeRepository(SchoolDbContext context) : base(context) { }

        public IEnumerable<Grade> GetGradesOrderedByLevel()
        {
            return _dbSet
                .OrderBy(g => g.Level)
                .ToList();
        }

        public Grade? GetHighestLevelGrade()
        {
            return _dbSet
                .OrderByDescending(g => g.Level)
                .FirstOrDefault();
        }

        public Grade? GetLowestLevelGrade()
        {
            return _dbSet
                .OrderBy(g => g.Level)
                .FirstOrDefault();
        }

        public Dictionary<string, int> GetClassCountByGradeName()
        {
            return _dbSet
                .ToDictionary(
                    g => g.Name,
                    g => _context.Classes.Count(c => c.GradeId == g.Id)
                );
        }

        public Dictionary<string, int> GetCourseCountByGradeName()
        {
            return _dbSet
                .ToDictionary(
                    g => g.Name,
                    g => _context.Courses.Count(c => c.GradeId == g.Id)
                );
        }

        public Dictionary<string, int> GetStudentCountByGradeName()
        {
            return _dbSet
                .ToDictionary(
                    g => g.Name,
                    g => _context.Classes
                        .Where(c => c.GradeId == g.Id)
                        .Sum(c => _context.Students.Count(s => s.ClassId == c.Id))
                );
        }
    }
}