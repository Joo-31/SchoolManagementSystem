using Microsoft.EntityFrameworkCore;
using SchoolManagementAPI.DataBase;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;

namespace SchoolManagementAPI.Repositories
{
    public class ClassRepository : Repository<Class>, IClassRepository
    {
        public ClassRepository(SchoolDbContext context) : base(context) { }

        // ✅ Override GetById
        public new Class? GetById(int id)
        {
            return _dbSet
                .Include(c => c.Grade)
                .Include(c => c.ClassTeacher)
                .FirstOrDefault(c => c.Id == id);
        }

        // ✅ Override GetAll
        public new IEnumerable<Class> GetAll()
        {
            return _dbSet
                .Include(c => c.Grade)
                .Include(c => c.ClassTeacher)
                .ToList();
        }

        public IEnumerable<Class> GetClassesOrderedByName()
        {
            return _dbSet
                .Include(c => c.Grade)
                .Include(c => c.ClassTeacher)
                .OrderBy(c => c.Name)
                .ToList();
        }

        public IEnumerable<Class> GetClassesByTeacherId(int teacherId)
        {
            return _dbSet
                .Include(c => c.Grade)
                .Include(c => c.ClassTeacher)
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