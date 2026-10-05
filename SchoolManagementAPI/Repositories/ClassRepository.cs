using Microsoft.EntityFrameworkCore;
using SchoolManagementAPI.DataBase;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;

namespace SchoolManagementAPI.Repositories
{
    public class ClassRepository : Repository<Class>, IClassRepository
    {
        public ClassRepository(SchoolDbContext context) : base(context)
        {
        }
        public new IEnumerable<Class> GetAll()
        {
            return _dbSet
                .Include(c => c.Grade)
                .Include(c => c.ClassTeacher)
                .ToList();
        }
        // ✅ Methods خاصة بالـ Class
        public IEnumerable<Class> GetClassesOrderedByName()
        {
            return _dbSet
                .OrderBy(c => c.Name)
                .ToList();
        }

        public IEnumerable<Class> GetClassesByTeacherId(int teacherId)
        {
            return _dbSet
                .Where(c => c.ClassTeacherId == teacherId)
                .ToList();
        }

        public Dictionary<int, int> GetClassCountByGrade()
        {
            return _dbSet
                .GroupBy(c => c.GradeId)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public Dictionary<string, int> GetStudentCountByClass()
        {
            return _dbSet
                .ToDictionary(
                    c => c.Name,
                    c => _context.Students.Count(s => s.ClassId == c.Id)
                );
        }

        public Class? GetClassWithMostStudents()
        {
            return _dbSet
                .AsEnumerable()
                .OrderByDescending(c => _context.Students.Count(s => s.ClassId == c.Id))
                .FirstOrDefault();
        }

        public Class? GetClassWithLeastStudents()
        {
            return _dbSet
                .AsEnumerable()
                .OrderBy(c => _context.Students.Count(s => s.ClassId == c.Id))
                .FirstOrDefault();
        }
    }
}