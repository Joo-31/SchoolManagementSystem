using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;
using SchoolManagementAPI.Services.Interfaces;
using SchoolManagementAPI.DTOs.Responses;
namespace SchoolManagementAPI.Services
{
    public class GradeService : IGradeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GradeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Add(Grade grade)
        {
            _unitOfWork.Grades.Add(grade);
            _unitOfWork.SaveChanges();
        }

        public List<Grade> GetAll()
        {
            return _unitOfWork.Grades.GetAll().ToList();
        }

        public Grade? GetById(int id)
        {
            return _unitOfWork.Grades.GetById(id);
        }

        public bool Update(Grade grade)
        {
            var existing = GetById(grade.Id);
            if (existing == null)
                return false;

            bool hasChanges =
                existing.Name != grade.Name ||
                existing.Level != grade.Level;

            if (!hasChanges)
                return false;

            existing.Name = grade.Name;
            existing.Level = grade.Level;

            _unitOfWork.Grades.Update(existing);
            _unitOfWork.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var grade = GetById(id);
            if (grade != null)
            {
                _unitOfWork.Grades.Delete(grade);
                _unitOfWork.SaveChanges();
                return true;
            }
            return false;
        }

        public List<Grade> Search(string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return GetAll();

            var lowerKeyword = keyword.ToLower();

            return _unitOfWork.Grades.Find(g => g.Name.ToLower().Contains(lowerKeyword)).ToList();
        }

        public List<Grade> GetGradesOrderedByLevel()
        {
            return _unitOfWork.Grades.GetGradesOrderedByLevel().ToList();
        }

        public Grade? GetHighestLevelGrade()
        {
            return _unitOfWork.Grades.GetHighestLevelGrade();
        }

        public Grade? GetLowestLevelGrade()
        {
            return _unitOfWork.Grades.GetLowestLevelGrade();
        }

        public Dictionary<string, int> GetClassCountByGradeName()
        {
            return _unitOfWork.Grades.GetClassCountByGradeName();
        }

        public Dictionary<string, int> GetCourseCountByGradeName()
        {
            return _unitOfWork.Grades.GetCourseCountByGradeName();
        }

        public Dictionary<string, int> GetStudentCountByGradeName()
        {
            return _unitOfWork.Grades.GetStudentCountByGradeName();
        }


        public PagedResult<Grade> GetPaged(int pageNumber, int pageSize)
        {
            return _unitOfWork.Grades.GetPaged(pageNumber, pageSize);
        }
    }
}