using SchoolManagementAPI.Models;

namespace SchoolManagementAPI.Repositories.Interfaces
{
    public interface IGradeRepository : IRepository<Grade>
    {
        // ✅ Methods خاصة بالـ Grade
        IEnumerable<Grade> GetGradesOrderedByLevel();
        Grade? GetHighestLevelGrade();
        Grade? GetLowestLevelGrade();
        Dictionary<string, int> GetClassCountByGradeName();
        Dictionary<string, int> GetCourseCountByGradeName();
        Dictionary<string, int> GetStudentCountByGradeName();
    }
}