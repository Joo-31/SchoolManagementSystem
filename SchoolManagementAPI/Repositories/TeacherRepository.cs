using SchoolManagementAPI.DataBase;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;

namespace SchoolManagementAPI.Repositories
{
    public class TeacherRepository : Repository<Teacher>, ITeacherRepository
    {
        public TeacherRepository(SchoolDbContext context) : base(context)
        {
        }

        // ✅ Methods خاصة بالـ Teacher
        public IEnumerable<Teacher> GetTeachersBySpecialization(string specialization)
        {
            return _dbSet
                .Where(t => t.Specialization == specialization)
                .ToList();
        }

        public IEnumerable<Teacher> GetTeachersOrderedByExperience()
        {
            return _dbSet
                .AsEnumerable()
                .OrderByDescending(t => t.GetYearsOfExperience())
                .ToList();
        }

        public IEnumerable<Teacher> GetTeachersWithExperienceMoreThan(int years)
        {
            return _dbSet
                .AsEnumerable()
                .Where(t => t.GetYearsOfExperience() > years)
                .ToList();
        }

        public Dictionary<string, int> GetTeacherCountBySpecialization()
        {
            return _dbSet
                .GroupBy(t => t.Specialization)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public double GetAverageExperience()
        {
            if (!_dbSet.Any())
                return 0;

            return _dbSet.AsEnumerable().Average(t => t.GetYearsOfExperience());
        }

        public Teacher? GetNewestTeacher()
        {
            return _dbSet
                .OrderByDescending(t => t.HireDate)
                .FirstOrDefault();
        }

        public Teacher? GetOldestTeacher()
        {
            return _dbSet
                .OrderBy(t => t.HireDate)
                .FirstOrDefault();
        }
    }
}