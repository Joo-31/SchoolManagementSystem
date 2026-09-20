
using SchoolManagementAPI.DataBase;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;

namespace SchoolManagementAPI.Repositories
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        public StudentRepository(SchoolDbContext context) : base(context)
        {
        }

        // ✅ Methods خاصة بالـ Student
        public IEnumerable<Student> GetStudentsByClassId(int classId)
        {
            return _dbSet.Where(s => s.ClassId == classId).ToList();
        }

        public IEnumerable<Student> GetStudentsOrderedByName()
        {
            return _dbSet
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToList();
        }

        public IEnumerable<Student> GetStudentsOlderThan(int age)
        {
            return _dbSet
                .AsEnumerable()
                .Where(s => s.GetAge() > age)
                .ToList();
        }

        public Dictionary<int, int> GetStudentCountByClass()
        {
            return _dbSet
                .GroupBy(s => s.ClassId)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public double GetAverageAge()
        {
            if (!_dbSet.Any())
                return 0;

            return _dbSet.AsEnumerable().Average(s => s.GetAge());
        }

        public Student? GetOldestStudent()
        {
            return _dbSet
                .AsEnumerable()
                .OrderByDescending(s => s.GetAge())
                .FirstOrDefault();
        }

        public Student? GetYoungestStudent()
        {
            return _dbSet
                .AsEnumerable()
                .OrderBy(s => s.GetAge())
                .FirstOrDefault();
        }
        // ✅ Methods جديدة
        public Student? GetOldestStudentByClass(int classId)
        {
            return _dbSet
                .Where(s => s.ClassId == classId)
                .AsEnumerable()
                .OrderByDescending(s => s.GetAge())
                .FirstOrDefault();
        }

        public Student? GetYoungestStudentByClass(int classId)
        {
            return _dbSet
                .Where(s => s.ClassId == classId)
                .AsEnumerable()
                .OrderBy(s => s.GetAge())
                .FirstOrDefault();
        }
    }
}