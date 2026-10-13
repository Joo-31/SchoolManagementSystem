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

        // Pagination
        PagedResult<Attendance> GetPaged(int pageNumber, int pageSize);

        // Pagination + Filters (جديد)
        PagedResult<Attendance> GetPagedWithFilters(
            int pageNumber,
            int pageSize,
            DateTime? date = null,
            int? classId = null,
            bool? isPresent = null);
    }
}