using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;
using SchoolManagementAPI.Services.Interfaces;
using SchoolManagementAPI.DTOs.Responses;
namespace SchoolManagementAPI.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Add(Attendance attendance)
        {
            _unitOfWork.Attendances.Add(attendance);
            _unitOfWork.SaveChanges();
        }

        public List<Attendance> GetAll()
        {
            return _unitOfWork.Attendances.GetAll().ToList();
        }

        public Attendance? GetById(int id)
        {
            return _unitOfWork.Attendances.GetById(id);
        }

        public bool Update(Attendance attendance)
        {
            var persistedAttendance = GetById(attendance.Id);
            if (persistedAttendance == null)
                return false;

            // ✅ غير القيم
            persistedAttendance.StudentId = attendance.StudentId;
            persistedAttendance.ClassId = attendance.ClassId;
            persistedAttendance.Date = attendance.Date;
            persistedAttendance.IsPresent = attendance.IsPresent;

            // ✅ احفظ دايماً (هو مش هيعمل حاجة لو مفيش تغيير فعلي)
            _unitOfWork.Attendances.Update(persistedAttendance);
            _unitOfWork.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var attendance = GetById(id);
            if (attendance != null)
            {
                _unitOfWork.Attendances.Delete(attendance);
                _unitOfWork.SaveChanges();
                return true;
            }
            return false;
        }

        public List<Attendance> SearchByStudentId(int studentId)
        {
            return _unitOfWork.Attendances.SearchByStudentId(studentId).ToList();
        }

        public List<Attendance> SearchByClassId(int classId)
        {
            return _unitOfWork.Attendances.SearchByClassId(classId).ToList();
        }

        public List<Attendance> SearchByDate(DateTime date)
        {
            return _unitOfWork.Attendances.SearchByDate(date).ToList();
        }

        public List<Attendance> GetAttendanceByStudentIdAndDate(int studentId, DateTime date)
        {
            return _unitOfWork.Attendances.GetAttendanceByStudentIdAndDate(studentId, date).ToList();
        }

        public int GetPresentCountByDate(DateTime date)
        {
            return _unitOfWork.Attendances.GetPresentCountByDate(date);
        }

        public int GetAbsentCountByDate(DateTime date)
        {
            return _unitOfWork.Attendances.GetAbsentCountByDate(date);
        }

        public double GetAttendancePercentageByDate(DateTime date)
        {
            return _unitOfWork.Attendances.GetAttendancePercentageByDate(date);
        }

        public int GetAbsentCountByStudent(int studentId)
        {
            return _unitOfWork.Attendances.GetAbsentCountByStudent(studentId);
        }

        public int? GetStudentWithMostAbsences()
        {
            return _unitOfWork.Attendances.GetStudentWithMostAbsences();
        }

        public Dictionary<string, int> GetAttendanceCountByClass()
        {
            return _unitOfWork.Attendances.GetAttendanceCountByClass();
        }
        public PagedResult<Attendance> GetPaged(int pageNumber, int pageSize)
        {
            return _unitOfWork.Attendances.GetPaged(pageNumber, pageSize);
        }
    }
}