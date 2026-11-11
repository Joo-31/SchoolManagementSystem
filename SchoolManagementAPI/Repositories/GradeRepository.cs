using SchoolManagementAPI.DataBase;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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
                .AsNoTracking()
                .Select(g => new
                {
                    g.Name,
                    Count = _context.Students.Count(s =>
                        _context.Classes.Any(c => c.Id == s.ClassId && c.GradeId == g.Id))
                })
                .ToDictionary(x => x.Name, x => x.Count);
        }
    }
}
