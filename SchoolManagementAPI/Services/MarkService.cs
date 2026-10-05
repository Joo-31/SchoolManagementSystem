using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;
using SchoolManagementAPI.Services.Interfaces;

namespace SchoolManagementAPI.Services
{
    public class MarkService : IMarkService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MarkService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Add(Mark mark)
        {
            _unitOfWork.Marks.Add(mark);
            _unitOfWork.SaveChanges();
        }

        public List<Mark> GetAll()
        {
            return _unitOfWork.Marks.GetAll().ToList();
        }

        public Mark? GetById(int id)
        {
            return _unitOfWork.Marks.GetById(id);
        }

        public bool Update(Mark mark)
        {
            var existing = GetById(mark.Id);
            if (existing == null) return false;

            existing.StudentId = mark.StudentId;
            existing.CourseId = mark.CourseId;
            existing.TeacherId = mark.TeacherId;
            existing.Score = mark.Score;
            existing.Date = mark.Date;
            existing.Notes = mark.Notes;

            _unitOfWork.Marks.Update(existing);
            _unitOfWork.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var mark = GetById(id);
            if (mark != null)
            {
                _unitOfWork.Marks.Delete(mark);
                _unitOfWork.SaveChanges();
                return true;
            }
            return false;
        }

        public List<Mark> GetByStudentId(int studentId)
        {
            return _unitOfWork.Marks.GetByStudentId(studentId).ToList();
        }

        public List<Mark> GetByCourseId(int courseId)
        {
            return _unitOfWork.Marks.GetByCourseId(courseId).ToList();
        }

        public List<Mark> GetByTeacherId(int teacherId)
        {
            return _unitOfWork.Marks.GetByTeacherId(teacherId).ToList();
        }
    }
}