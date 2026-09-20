using SchoolManagementAPI.Models;
using SchoolManagementAPI.DTOs.Responses;
namespace SchoolManagementAPI.Services.Interfaces
{
    public interface IAttendanceService
    {
        // CRUD
        void Add(Attendance attendance);
        List<Attendance> GetAll();
        Attendance? GetById(int id);
        bool Update(Attendance attendance);
        bool Delete(int id);

        // Search & Filter
        List<Attendance> SearchByStudentId(int studentId);
        List<Attendance> SearchByClassId(int classId);
        List<Attendance> SearchByDate(DateTime date);
        List<Attendance> GetAttendanceByStudentIdAndDate(int studentId, DateTime date);

        // Statistics
        int GetPresentCountByDate(DateTime date);
        int GetAbsentCountByDate(DateTime date);
        double GetAttendancePercentageByDate(DateTime date);
        int GetAbsentCountByStudent(int studentId);
        int? GetStudentWithMostAbsences();
        Dictionary<string, int> GetAttendanceCountByClass();
        PagedResult<Attendance> GetPaged(int pageNumber, int pageSize);

    }
}