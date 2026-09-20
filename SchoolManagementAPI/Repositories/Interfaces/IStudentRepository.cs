using SchoolManagementAPI.Models;

namespace SchoolManagementAPI.Repositories.Interfaces
{
    public interface IStudentRepository : IRepository<Student>
    {
        // ✅ Methods خاصة بالـ Student بس
        IEnumerable<Student> GetStudentsByClassId(int classId);
        IEnumerable<Student> GetStudentsOrderedByName();
        IEnumerable<Student> GetStudentsOlderThan(int age);
        Dictionary<int, int> GetStudentCountByClass();
        double GetAverageAge();
        Student? GetOldestStudent();
        Student? GetYoungestStudent();
        // ✅ Methods جديدة
        Student? GetOldestStudentByClass(int classId);
        Student? GetYoungestStudentByClass(int classId);    
    }
}