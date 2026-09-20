using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;
using SchoolManagementAPI.Services.Interfaces;
using SchoolManagementAPI.DTOs.Responses;
namespace SchoolManagementAPI.Services
{
    public class ClassService : IClassService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClassService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Add(Class @class)
        {
            _unitOfWork.Classes.Add(@class);
            _unitOfWork.SaveChanges();
        }

        public List<Class> GetAll()
        {
            return _unitOfWork.Classes.GetAll().ToList();
        }

        public Class? GetById(int id)
        {
            return _unitOfWork.Classes.GetById(id);
        }

        public bool Update(Class @class)
        {
            var existing = GetById(@class.Id);
            if (existing == null)
                return false;

            bool hasChanges =
                existing.Name != @class.Name ||
                existing.GradeId != @class.GradeId ||
                existing.ClassTeacherId != @class.ClassTeacherId;

            if (!hasChanges)
                return false;

            existing.Name = @class.Name;
            existing.GradeId = @class.GradeId;
            existing.ClassTeacherId = @class.ClassTeacherId;

            _unitOfWork.Classes.Update(existing);
            _unitOfWork.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var @class = GetById(id);
            if (@class != null)
            {
                _unitOfWork.Classes.Delete(@class);
                _unitOfWork.SaveChanges();
                return true;
            }
            return false;
        }

        public List<Class> Search(string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return GetAll();

            var lowerKeyword = keyword.ToLower();

            return _unitOfWork.Classes.Find(c => c.Name.ToLower().Contains(lowerKeyword)).ToList();
        }

        public List<Class> GetClassesOrderedByName()
        {
            return _unitOfWork.Classes.GetClassesOrderedByName().ToList();
        }

        public List<Class> GetClassesByTeacherId(int teacherId)
        {
            return _unitOfWork.Classes.GetClassesByTeacherId(teacherId).ToList();
        }

        public Dictionary<int, int> GetClassCountByGrade()
        {
            return _unitOfWork.Classes.GetClassCountByGrade();
        }

        public Dictionary<string, int> GetStudentCountByClass()
        {
            return _unitOfWork.Classes.GetStudentCountByClass();
        }

        public Class? GetClassWithMostStudents()
        {
            return _unitOfWork.Classes.GetClassWithMostStudents();
        }

        public Class? GetClassWithLeastStudents()
        {
            return _unitOfWork.Classes.GetClassWithLeastStudents();
        }

        public PagedResult<Class> GetPaged(int pageNumber, int pageSize)
        {
            return _unitOfWork.Classes.GetPaged(pageNumber, pageSize);
        }
    }
}