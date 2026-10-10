using SchoolManagementAPI.DTOs.Responses;
using SchoolManagementAPI.Models;

namespace SchoolManagementAPI.Repositories.Interfaces
{
    public interface ICourseRepository : IRepository<Course>
    {
        // ✅ Methods خاصة بالـ Course
        IEnumerable<Course> GetCoursesByGradeId(int gradeId);
        IEnumerable<Course> GetCoursesOrderedByCredits();
        IEnumerable<Course> GetCoursesWithCreditsMoreThan(int credits);
        Dictionary<int, int> GetCourseCountByGrade();
        double GetAverageCredits();
        Course? GetCourseWithMaxCredits();
        Course? GetCourseWithMinCredits();

        new PagedResult<Course> GetPaged(int pageNumber, int pageSize);
    }
}