using SchoolManagementAPI.Models;
using SchoolManagementAPI.DTOs.Responses;
namespace SchoolManagementAPI.Services.Interfaces
{
    public interface ITeacherService
    {
        // CRUD
        void Add(Teacher teacher);
        List<Teacher> GetAll();
        Teacher? GetById(int id);
        bool Update(Teacher teacher);
        bool Delete(int id);

        // Search & Filter
        List<Teacher> Search(string? keyword);
        List<Teacher> GetTeachersBySpecialization(string? specialization);
        List<Teacher> GetTeachersOrderedByExperience();
        List<Teacher> GetTeachersWithExperienceMoreThan(int years);

        // Statistics
        Dictionary<string, int> GetTeacherCountBySpecialization();
        double GetAverageExperience();
        Teacher? GetNewestTeacher();
        Teacher? GetOldestTeacher();


        PagedResult<Teacher> GetPaged(int pageNumber, int pageSize);
    }
}