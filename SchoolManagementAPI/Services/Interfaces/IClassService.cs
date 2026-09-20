using SchoolManagementAPI.Models;
using SchoolManagementAPI.DTOs.Responses;
namespace SchoolManagementAPI.Services.Interfaces
{
    public interface IClassService
    {
        // CRUD
        void Add(Class @class);
        List<Class> GetAll();
        Class? GetById(int id);
        bool Update(Class @class);
        bool Delete(int id);

        // Search & Filter
        List<Class> Search(string? keyword);
        List<Class> GetClassesOrderedByName();
        List<Class> GetClassesByTeacherId(int teacherId);

        // Statistics
        Dictionary<int, int> GetClassCountByGrade();
        Dictionary<string, int> GetStudentCountByClass();
        Class? GetClassWithMostStudents();
        Class? GetClassWithLeastStudents();

        PagedResult<Class> GetPaged(int pageNumber, int pageSize);
    }
}