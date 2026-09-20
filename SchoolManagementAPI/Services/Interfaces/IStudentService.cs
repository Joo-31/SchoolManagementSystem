using SchoolManagementAPI.Models;
using SchoolManagementAPI.DTOs.Responses;

namespace SchoolManagementAPI.Services.Interfaces
{
    public interface IStudentService
    {
        // CRUD
        void Add(Student student);
        List<Student> GetAll();
        Student? GetById(int id);
        bool Update(Student student);
        bool Delete(int id);

        // Search & Filter
        List<Student> Search(string? keyword);
        List<Student> GetStudentsByClassId(int classId);
        List<Student> GetStudentsOrderedByName();
        List<Student> GetStudentsOlderThan(int age);

        // Statistics
        Dictionary<int, int> GetStudentCountByClass();
        double GetAverageAge();
        Student? GetOldestStudent();
        Student? GetYoungestStudent();

        PagedResult<Student> GetPaged(int pageNumber, int pageSize);

        // ✅ Methods جديدة
        Student? GetOldestStudentByClass(int classId);
        Student? GetYoungestStudentByClass(int classId);
    }
}