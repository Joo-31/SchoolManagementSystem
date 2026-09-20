using SchoolManagementAPI.Models;

namespace SchoolManagementAPI.Repositories.Interfaces
{
    public interface ITeacherRepository : IRepository<Teacher>
    {
        // ✅ Methods خاصة بالـ Teacher
        IEnumerable<Teacher> GetTeachersBySpecialization(string specialization);
        IEnumerable<Teacher> GetTeachersOrderedByExperience();
        IEnumerable<Teacher> GetTeachersWithExperienceMoreThan(int years);
        Dictionary<string, int> GetTeacherCountBySpecialization();
        double GetAverageExperience();
        Teacher? GetNewestTeacher();
        Teacher? GetOldestTeacher();
    }
}