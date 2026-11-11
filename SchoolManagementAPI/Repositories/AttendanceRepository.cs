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
            return _dbSet.Count(a => a.Date.Date == date.Date && a.IsPresent);
        }

        public int GetAbsentCountByDate(DateTime date)
        {
            return _dbSet.Count(a => a.Date.Date == date.Date && !a.IsPresent);
        }

        // ✅ بنجيب النسبة بطريقة سريعة جداً
        public double GetAttendancePercentageByDate(DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1);

            // Aggregate in SQL and use a half-open range so an index on Date can be used.
            var counts = _dbSet
                .AsNoTracking()
                .Where(a => a.Date >= start && a.Date < end)
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Present = g.Count(a => a.IsPresent)
                })
                .FirstOrDefault();

            return counts is null || counts.Total == 0
                ? 0
                : (double)counts.Present / counts.Total * 100;
        }

        public int GetAbsentCountByStudent(int studentId)
        {
            return _dbSet.Count(a => a.StudentId == studentId && !a.IsPresent);
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

        // ✅ GetPaged العادي
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

        // ✅ GetPagedWithFilters
        public PagedResult<Attendance> GetPagedWithFilters(
            int pageNumber,
            int pageSize,
            DateTime? date = null,
            int? classId = null,
            bool? isPresent = null)
        {
            var query = _dbSet
                .Include(a => a.Student)
                .Include(a => a.Class)
                .AsNoTracking()
                .AsQueryable();

            if (date.HasValue)
            {
                var targetDate = date.Value.Date;
                query = query.Where(a => a.Date.Date == targetDate);
            }

            if (classId.HasValue && classId.Value > 0)
            {
                query = query.Where(a => a.ClassId == classId.Value);
            }

            if (isPresent.HasValue)
            {
                query = query.Where(a => a.IsPresent == isPresent.Value);
            }

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
