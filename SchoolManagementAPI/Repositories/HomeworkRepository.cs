using Microsoft.EntityFrameworkCore;
using SchoolManagementAPI.DataBase;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SchoolManagementAPI.Repositories
{
    public class HomeworkRepository : Repository<Homework>, IHomeworkRepository
    {
        public HomeworkRepository(SchoolDbContext context) : base(context) { }

        public new Homework? GetById(int id)
        {
            return _dbSet
                .Include(h => h.Course)
                .Include(h => h.Teacher)
                .FirstOrDefault(h => h.Id == id);
        }

        public new IEnumerable<Homework> GetAll()
        {
            return _dbSet
                .Include(h => h.Course)
                .Include(h => h.Teacher)
                .ToList();
        }

        public IEnumerable<Homework> GetByTeacherId(int teacherId)
        {
            return _dbSet
                .Include(h => h.Course)
                .Include(h => h.Teacher)
                .Where(h => h.TeacherId == teacherId)
                .OrderByDescending(h => h.CreatedAt)
                .ToList();
        }

        public IEnumerable<Homework> GetByStudentId(int studentId)
        {
            // بنجيب الكورسات اللي الطالب ده مسجل فيها (عن طريق الفصل بتاعه)
            var student = _context.Students.Include(s => s.Class).FirstOrDefault(s => s.Id == studentId);
            if (student == null || student.Class == null) return new List<Homework>();

            var studentGradeId = student.Class.GradeId;

            return _dbSet
                .Include(h => h.Course)
                .Include(h => h.Teacher)
                .Where(h => h.Course != null && h.Course.GradeId == studentGradeId) // بنجيب واجبات المواد اللي في مرحلته
                .OrderByDescending(h => h.CreatedAt)
                .ToList();
        }
    }
}