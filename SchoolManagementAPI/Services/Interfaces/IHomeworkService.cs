using SchoolManagementAPI.DTOs.Requests;
using SchoolManagementAPI.DTOs.Responses;
using System.Collections.Generic;

namespace SchoolManagementAPI.Services.Interfaces
{
    public interface IHomeworkService
    {
        void Create(CreateHomeworkDto dto, int teacherId);
        List<HomeworkDto> GetByTeacherId(int teacherId);
        List<HomeworkDto> GetByStudentId(int studentId);
        void Delete(int id, int? teacherId = null); // لو teacherId موجود، يبقى بيتأكد إنه صاحب الواجب
        HomeworkDto? GetById(int id);
    }
}