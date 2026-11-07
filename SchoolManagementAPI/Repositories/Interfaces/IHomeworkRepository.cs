using SchoolManagementAPI.Models;
using System.Collections.Generic;

namespace SchoolManagementAPI.Repositories.Interfaces
{
    public interface IHomeworkRepository : IRepository<Homework>
    {
        IEnumerable<Homework> GetByTeacherId(int teacherId);
        IEnumerable<Homework> GetByStudentId(int studentId);
    }
}