using SchoolManagementAPI.DTOs.Responses;
using SchoolManagementAPI.Models;

namespace SchoolManagementAPI.Repositories.Interfaces
{
    public interface IMarkRepository : IRepository<Mark>
    {
        IEnumerable<Mark> GetByStudentId(int studentId);
        IEnumerable<Mark> GetByCourseId(int courseId);
        IEnumerable<Mark> GetByTeacherId(int teacherId);

        // ✅ ضيف دي
        PagedResult<Mark> GetPaged(int pageNumber, int pageSize, int? teacherId = null);
    }
}