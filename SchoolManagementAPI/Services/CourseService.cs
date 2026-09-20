using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;
using SchoolManagementAPI.Services.Interfaces;
using SchoolManagementAPI.DTOs.Responses;

namespace SchoolManagementAPI.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Add(Course course)
        {
            _unitOfWork.Courses.Add(course);
            _unitOfWork.SaveChanges();
        }

        public List<Course> GetAll()
        {
            return _unitOfWork.Courses.GetAll().ToList();
        }

        public Course? GetById(int id)
        {
            return _unitOfWork.Courses.GetById(id);
        }

        public bool Update(Course course)
        {
            var existing = GetById(course.Id);
            if (existing == null)
                return false;

            bool hasChanges =
                existing.Name != course.Name ||
                existing.Code != course.Code ||
                existing.Credits != course.Credits ||
                existing.GradeId != course.GradeId;

            if (!hasChanges)
                return false;

            existing.Name = course.Name;
            existing.Code = course.Code;
            existing.Credits = course.Credits;
            existing.GradeId = course.GradeId;

            _unitOfWork.Courses.Update(existing);
            _unitOfWork.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var course = GetById(id);
            if (course != null)
            {
                _unitOfWork.Courses.Delete(course);
                _unitOfWork.SaveChanges();
                return true;
            }
            return false;
        }

        public List<Course> Search(string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return GetAll();

            var lowerKeyword = keyword.ToLower();

            return _unitOfWork.Courses.Find(c =>
                c.Name.ToLower().Contains(lowerKeyword) ||
                c.Code.ToLower().Contains(lowerKeyword)
            ).ToList();
        }

        public List<Course> GetCoursesByGradeId(int gradeId)
        {
            return _unitOfWork.Courses.GetCoursesByGradeId(gradeId).ToList();
        }

        public List<Course> GetCoursesOrderedByCredits()
        {
            return _unitOfWork.Courses.GetCoursesOrderedByCredits().ToList();
        }

        public List<Course> GetCoursesWithCreditsMoreThan(int credits)
        {
            return _unitOfWork.Courses.GetCoursesWithCreditsMoreThan(credits).ToList();
        }

        public Dictionary<int, int> GetCourseCountByGrade()
        {
            return _unitOfWork.Courses.GetCourseCountByGrade();
        }

        public double GetAverageCredits()
        {
            return _unitOfWork.Courses.GetAverageCredits();
        }

        public Course? GetCourseWithMaxCredits()
        {
            return _unitOfWork.Courses.GetCourseWithMaxCredits();
        }

        public Course? GetCourseWithMinCredits()
        {
            return _unitOfWork.Courses.GetCourseWithMinCredits();
        }

        public PagedResult<Course> GetPaged(int pageNumber, int pageSize)
        {
            return _unitOfWork.Courses.GetPaged(pageNumber, pageSize);
        }
    }
}