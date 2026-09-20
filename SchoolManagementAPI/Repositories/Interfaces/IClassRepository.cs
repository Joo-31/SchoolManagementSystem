using SchoolManagementAPI.Models;

namespace SchoolManagementAPI.Repositories.Interfaces
{
    public interface IClassRepository : IRepository<Class>
    {
        // ✅ Methods خاصة بالـ Class
        IEnumerable<Class> GetClassesOrderedByName();
        IEnumerable<Class> GetClassesByTeacherId(int teacherId);
        Dictionary<int, int> GetClassCountByGrade();
        Dictionary<string, int> GetStudentCountByClass();
        Class? GetClassWithMostStudents();
        Class? GetClassWithLeastStudents();
    }
}