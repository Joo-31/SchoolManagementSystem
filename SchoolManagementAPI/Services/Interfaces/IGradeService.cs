using SchoolManagementAPI.Models;
using SchoolManagementAPI.DTOs.Responses;
namespace SchoolManagementAPI.Services.Interfaces
{
    public interface IGradeService
    {
        // CRUD
        void Add(Grade grade);
        List<Grade> GetAll();
        Grade? GetById(int id);
        bool Update(Grade grade);
        bool Delete(int id);

        // Search & Filter
        List<Grade> Search(string? keyword);
        List<Grade> GetGradesOrderedByLevel();

        // Statistics
        Grade? GetHighestLevelGrade();
        Grade? GetLowestLevelGrade();
        Dictionary<string, int> GetClassCountByGradeName();
        Dictionary<string, int> GetCourseCountByGradeName();
        Dictionary<string, int> GetStudentCountByGradeName();

        PagedResult<Grade> GetPaged(int pageNumber, int pageSize);
    }
}