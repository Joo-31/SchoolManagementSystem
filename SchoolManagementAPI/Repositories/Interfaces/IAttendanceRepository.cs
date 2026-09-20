using SchoolManagementAPI.Models;

namespace SchoolManagementAPI.Repositories.Interfaces
{
    public interface IAttendanceRepository : IRepository<Attendance>
    {
        // ✅ Methods خاصة بالـ Attendance
        IEnumerable<Attendance> SearchByStudentId(int studentId);
        IEnumerable<Attendance> SearchByClassId(int classId);
        IEnumerable<Attendance> SearchByDate(DateTime date);
        IEnumerable<Attendance> GetAttendanceByStudentIdAndDate(int studentId, DateTime date);
        int GetPresentCountByDate(DateTime date);
        int GetAbsentCountByDate(DateTime date);
        double GetAttendancePercentageByDate(DateTime date);
        int GetAbsentCountByStudent(int studentId);
        int? GetStudentWithMostAbsences();
        Dictionary<string, int> GetAttendanceCountByClass();
    }
}