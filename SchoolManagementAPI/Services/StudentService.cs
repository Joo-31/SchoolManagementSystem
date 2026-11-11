using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;
using SchoolManagementAPI.Services.Interfaces;
using SchoolManagementAPI.DTOs.Responses;

namespace SchoolManagementAPI.Services
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ✅ CRUD
        public void Add(Student student, string username, string password)
        {
            // 1. نحفظ الطالب الأول عشان ناخد الـ ID بتاعه
            _unitOfWork.Students.Add(student);
            _unitOfWork.SaveChanges();

            // 2. نعمل الـ User ونربطه بالطالب
            var user = new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "Student",
                StudentId = student.Id,
                CreatedAt = DateTime.Now
            };
            _unitOfWork.Users.Add(user);
            _unitOfWork.SaveChanges();
        }

        public List<Student> GetAll()
        {
            return _unitOfWork.Students.GetAll().ToList();
        }

        public Student? GetById(int id)
        {
            return _unitOfWork.Students.GetById(id);
        }

        public bool Update(Student student)
        {
            var existing = _unitOfWork.Students.GetById(student.Id);
            if (existing == null)
                return false;

            existing.FirstName = student.FirstName;
            existing.LastName = student.LastName;
            existing.BirthDate = student.BirthDate;
            existing.Gender = student.Gender;
            existing.ClassId = student.ClassId;

            _unitOfWork.Students.Update(existing);
            _unitOfWork.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var student = GetById(id);
            if (student != null)
            {
                // ✅ الأول بنمسح الـ User المرتبط بالطالب ده
                var user = _unitOfWork.Users.Find(u => u.StudentId == id).FirstOrDefault();
                if (user != null)
                {
                    _unitOfWork.Users.Delete(user);
                }

                // ✅ بعدين بنمسح الطالب نفسه
                _unitOfWork.Students.Delete(student);
                _unitOfWork.SaveChanges();
                return true;
            }
            return false;
        }

        // ✅ Search & Filter
        public List<Student> Search(string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return GetAll();

            return _unitOfWork.Students.Search(keyword).ToList();
        }

        public List<Student> GetStudentsByClassId(int classId)
        {
            return _unitOfWork.Students.GetStudentsByClassId(classId).ToList();
        }

        public List<Student> GetStudentsOrderedByName()
        {
            return _unitOfWork.Students.GetStudentsOrderedByName().ToList();
        }

        public List<Student> GetStudentsOlderThan(int age)
        {
            return _unitOfWork.Students.GetStudentsOlderThan(age).ToList();
        }

        // ✅ Statistics
        public Dictionary<int, int> GetStudentCountByClass()
        {
            return _unitOfWork.Students.GetStudentCountByClass();
        }

        public double GetAverageAge()
        {
            return _unitOfWork.Students.GetAverageAge();
        }

        public Student? GetOldestStudent()
        {
            return _unitOfWork.Students.GetOldestStudent();
        }

        public Student? GetYoungestStudent()
        {
            return _unitOfWork.Students.GetYoungestStudent();
        }
        public PagedResult<Student> GetPaged(int pageNumber, int pageSize)
        {
            return _unitOfWork.Students.GetPaged(pageNumber, pageSize);
        }
        // ✅ Methods جديدة
        public Student? GetOldestStudentByClass(int classId)
        {
            return _unitOfWork.Students.GetOldestStudentByClass(classId);
        }

        public Student? GetYoungestStudentByClass(int classId)
        {
            return _unitOfWork.Students.GetYoungestStudentByClass(classId);
        }
    }
}