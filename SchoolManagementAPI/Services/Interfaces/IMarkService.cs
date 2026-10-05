using SchoolManagementAPI.Models;

namespace SchoolManagementAPI.Services.Interfaces
{
    public interface IMarkService
    {
        void Add(Mark mark);
        List<Mark> GetAll();
        Mark? GetById(int id);
        bool Update(Mark mark);
        bool Delete(int id);
        List<Mark> GetByStudentId(int studentId);
        List<Mark> GetByCourseId(int courseId);
        List<Mark> GetByTeacherId(int teacherId);
    }
}