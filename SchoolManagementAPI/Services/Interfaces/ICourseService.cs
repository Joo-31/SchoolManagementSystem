using SchoolManagementAPI.Models;
using SchoolManagementAPI.DTOs.Responses;

namespace SchoolManagementAPI.Services.Interfaces
{
    public interface ICourseService
    {
        // CRUD
        void Add(Course course);
        List<Course> GetAll();
        Course? GetById(int id);
        bool Update(Course course);
        bool Delete(int id);

        // Search & Filter
        List<Course> Search(string? keyword);
        List<Course> GetCoursesByGradeId(int gradeId);
        List<Course> GetCoursesOrderedByCredits();
        List<Course> GetCoursesWithCreditsMoreThan(int credits);

        // Statistics
        Dictionary<int, int> GetCourseCountByGrade();
        double GetAverageCredits();
        Course? GetCourseWithMaxCredits();
        Course? GetCourseWithMinCredits();


        PagedResult<Course> GetPaged(int pageNumber, int pageSize);
    }
}