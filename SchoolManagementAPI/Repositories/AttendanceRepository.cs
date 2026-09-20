using SchoolManagementAPI.DataBase;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;

namespace SchoolManagementAPI.Repositories
{
    public class AttendanceRepository : Repository<Attendance>, IAttendanceRepository
    {
        public AttendanceRepository(SchoolDbContext context) : base(context)
        {
        }

        // ✅ Methods خاصة بالـ Attendance
        public IEnumerable<Attendance> SearchByStudentId(int studentId)
        {
            return _dbSet.Where(a => a.StudentId == studentId).ToList();
        }

        public IEnumerable<Attendance> SearchByClassId(int classId)
        {
            return _dbSet.Where(a => a.ClassId == classId).ToList();
        }

        public IEnumerable<Attendance> SearchByDate(DateTime date)
        {
            return _dbSet.Where(a => a.Date.Date == date.Date).ToList();
        }

        public IEnumerable<Attendance> GetAttendanceByStudentIdAndDate(int studentId, DateTime date)
        {
            return _dbSet
                .Where(a => a.StudentId == studentId && a.Date.Date == date.Date)
                .ToList();
        }

        public int GetPresentCountByDate(DateTime date)
        {
            return _dbSet
                .Count(a => a.Date.Date == date.Date && a.IsPresent);
        }

        public int GetAbsentCountByDate(DateTime date)
        {
            return _dbSet
                .Count(a => a.Date.Date == date.Date && !a.IsPresent);
        }

        public double GetAttendancePercentageByDate(DateTime date)
        {
            var total = _dbSet.Count(a => a.Date.Date == date.Date);
            if (total == 0) return 0;

            var present = _dbSet.Count(a => a.Date.Date == date.Date && a.IsPresent);
            return (double)present / total * 100;
        }

        public int GetAbsentCountByStudent(int studentId)
        {
            return _dbSet
                .Count(a => a.StudentId == studentId && !a.IsPresent);
        }

        public int? GetStudentWithMostAbsences()
        {
            return _dbSet
                .Where(a => !a.IsPresent)
                .GroupBy(a => a.StudentId)
                .OrderByDescending(g => g.Count())
                .Select(g => (int?)g.Key)
                .FirstOrDefault();
        }

        public Dictionary<string, int> GetAttendanceCountByClass()
        {
            return _context.Classes
                .ToDictionary(
                    c => c.Name,
                    c => _context.Attendances.Count(a => a.ClassId == c.Id && a.IsPresent)
                );
        }
    }
}