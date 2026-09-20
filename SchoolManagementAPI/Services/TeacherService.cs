using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;
using SchoolManagementAPI.Services.Interfaces;
using SchoolManagementAPI.DTOs.Responses;
namespace SchoolManagementAPI.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TeacherService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Add(Teacher teacher)
        {
            _unitOfWork.Teachers.Add(teacher);
            _unitOfWork.SaveChanges();
        }

        public List<Teacher> GetAll()
        {
            return _unitOfWork.Teachers.GetAll().ToList();
        }

        public Teacher? GetById(int id)
        {
            return _unitOfWork.Teachers.GetById(id);
        }

        public bool Update(Teacher teacher)
        {
            var existing = GetById(teacher.Id);
            if (existing == null)
                return false;

            bool hasChanges =
                existing.Name != teacher.Name ||
                existing.Email != teacher.Email ||
                existing.Phone != teacher.Phone ||
                existing.Specialization != teacher.Specialization ||
                existing.HireDate != teacher.HireDate;

            if (!hasChanges)
                return false;

            existing.Name = teacher.Name;
            existing.Email = teacher.Email;
            existing.Phone = teacher.Phone;
            existing.Specialization = teacher.Specialization;
            existing.HireDate = teacher.HireDate;

            _unitOfWork.Teachers.Update(existing);
            _unitOfWork.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var teacher = GetById(id);
            if (teacher != null)
            {
                _unitOfWork.Teachers.Delete(teacher);
                _unitOfWork.SaveChanges();
                return true;
            }
            return false;
        }

        public List<Teacher> Search(string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return GetAll();

            var lowerKeyword = keyword.ToLower();

            return _unitOfWork.Teachers.Find(t =>
                t.Name.ToLower().Contains(lowerKeyword) ||
                t.Specialization.ToLower().Contains(lowerKeyword)
            ).ToList();
        }

        public List<Teacher> GetTeachersBySpecialization(string? specialization)
        {
            if (string.IsNullOrWhiteSpace(specialization))
                return GetAll();

            return _unitOfWork.Teachers.GetTeachersBySpecialization(specialization).ToList();
        }

        public List<Teacher> GetTeachersOrderedByExperience()
        {
            return _unitOfWork.Teachers.GetTeachersOrderedByExperience().ToList();
        }

        public List<Teacher> GetTeachersWithExperienceMoreThan(int years)
        {
            return _unitOfWork.Teachers.GetTeachersWithExperienceMoreThan(years).ToList();
        }

        public Dictionary<string, int> GetTeacherCountBySpecialization()
        {
            return _unitOfWork.Teachers.GetTeacherCountBySpecialization();
        }

        public double GetAverageExperience()
        {
            return _unitOfWork.Teachers.GetAverageExperience();
        }

        public Teacher? GetNewestTeacher()
        {
            return _unitOfWork.Teachers.GetNewestTeacher();
        }

        public Teacher? GetOldestTeacher()
        {
            return _unitOfWork.Teachers.GetOldestTeacher();
        }

        public PagedResult<Teacher> GetPaged(int pageNumber, int pageSize)
        {
            return _unitOfWork.Teachers.GetPaged(pageNumber, pageSize);
        }
    }
}