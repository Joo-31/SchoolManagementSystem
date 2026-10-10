using Microsoft.EntityFrameworkCore;
using SchoolManagementAPI.DataBase;
using SchoolManagementAPI.DTOs.Responses;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;

namespace SchoolManagementAPI.Repositories
{
    public class AttendanceRepository : Repository<Attendance>, IAttendanceRepository
    {
        public AttendanceRepository(SchoolDbContext context) : base(context) { }

        // ✅ Override GetById
        public new Attendance? GetById(int id)
        {
            return _dbSet
                .Include(a => a.Student)
                .Include(a => a.Class)
                .FirstOrDefault(a => a.Id == id);
        }

        // ✅ Override GetAll
        public new IEnumerable<Attendance> GetAll()
        {
            return _dbSet
                .Include(a => a.Student)
                .Include(a => a.Class)
                .ToList();
        }

        public IEnumerable<Attendance> SearchByStudentId(int studentId)
        {
            return _dbSet
                .Include(a => a.Student)
                .Include(a => a.Class)
                .Where(a => a.StudentId == studentId)
                .ToList();
        }

        public IEnumerable<Attendance> SearchByClassId(int classId)
        {
            return _dbSet
                .Include(a => a.Student)
                .Include(a => a.Class)
                .Where(a => a.ClassId == classId)
                .ToList();
        }

        public IEnumerable<Attendance> SearchByDate(DateTime date)
        {
            return _dbSet
                .Include(a => a.Student)
                .Include(a => a.Class)
                .Where(a => a.Date.Date == date.Date)
                .ToList();
        }

        public IEnumerable<Attendance> GetAttendanceByStudentIdAndDate(int studentId, DateTime date)
        {
            return _dbSet
                .Include(a => a.Student)
                .Include(a => a.Class)
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

        // ✅ Override GetPaged عشان نعمل Include
        public new PagedResult<Attendance> GetPaged(int pageNumber, int pageSize)
        {
            var query = _dbSet
                .Include(a => a.Student)
                .Include(a => a.Class)
                .AsNoTracking()
                .AsQueryable();

            var totalCount = query.Count();

            var data = query
                .OrderByDescending(a => a.Date)
                .ThenBy(a => a.StudentId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<Attendance>
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