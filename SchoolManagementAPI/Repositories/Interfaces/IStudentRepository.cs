using SchoolManagementAPI.DTOs.Responses;
using SchoolManagementAPI.Models;

namespace SchoolManagementAPI.Repositories.Interfaces
{
    public interface IStudentRepository : IRepository<Student>
    {
        // ✅ Methods خاصة بالـ Student بس
        IEnumerable<Student> GetStudentsByClassId(int classId);
        IEnumerable<Student> GetStudentsByClassIds(IReadOnlyCollection<int> classIds);
        IEnumerable<Student> GetStudentsOrderedByName();
        IEnumerable<Student> GetStudentsOlderThan(int age);
        IEnumerable<Student> Search(string keyword);

        Dictionary<int, int> GetStudentCountByClass();
        double GetAverageAge();
        Student? GetOldestStudent();
        Student? GetYoungestStudent();
        // ✅ Methods جديدة
        Student? GetOldestStudentByClass(int classId);
        Student? GetYoungestStudentByClass(int classId);


        new PagedResult<Student> GetPaged(int pageNumber, int pageSize);
    }
}
