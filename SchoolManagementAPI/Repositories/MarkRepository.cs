using Microsoft.EntityFrameworkCore;
using SchoolManagementAPI.DataBase;
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
    }
}