using FluentAssertions;
using Moq;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;
using SchoolManagementAPI.Services;
using Xunit;

namespace SchoolManagementAPI.Tests
{
    public class TeacherServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ITeacherRepository> _teacherRepoMock;
        private readonly TeacherService _teacherService;

        public TeacherServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _teacherRepoMock = new Mock<ITeacherRepository>();
            _unitOfWorkMock.Setup(u => u.Teachers).Returns(_teacherRepoMock.Object);
            _teacherService = new TeacherService(_unitOfWorkMock.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnAllTeachers()
        {
            var teachers = new List<Teacher>
            {
                new Teacher { Id = 1, Name = "Ahmed Mohamed", Email = "ahmed@test.com", Phone = "0100", Specialization = "Math", HireDate = DateTime.Now.AddYears(-5) },
                new Teacher { Id = 2, Name = "Sara Ali", Email = "sara@test.com", Phone = "0200", Specialization = "English", HireDate = DateTime.Now.AddYears(-3) }
            };
            _teacherRepoMock.Setup(r => r.GetAll()).Returns(teachers);

            var result = _teacherService.GetAll();

            result.Should().HaveCount(2);
        }

        [Fact]
        public void GetById_ShouldReturnTeacher_WhenExists()
        {
            var teacher = new Teacher { Id = 1, Name = "Ahmed Mohamed" };
            _teacherRepoMock.Setup(r => r.GetById(1)).Returns(teacher);

            var result = _teacherService.GetById(1);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Ahmed Mohamed");
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenNotExists()
        {
            _teacherRepoMock.Setup(r => r.GetById(99)).Returns((Teacher?)null);

            var result = _teacherService.GetById(99);

            result.Should().BeNull();
        }

        [Fact]
        public void Add_ShouldAddTeacherAndSaveChanges()
        {
            var teacher = new Teacher { Id = 1, Name = "Ahmed", Email = "a@t.com", Phone = "0100", Specialization = "Math", HireDate = DateTime.Now };

            _teacherService.Add(teacher);

            _teacherRepoMock.Verify(r => r.Add(teacher), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Delete_ShouldDeleteTeacher_WhenExists()
        {
            var teacher = new Teacher { Id = 1, Name = "Ahmed" };
            _teacherRepoMock.Setup(r => r.GetById(1)).Returns(teacher);

            var result = _teacherService.Delete(1);

            result.Should().BeTrue();
            _teacherRepoMock.Verify(r => r.Delete(teacher), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Delete_ShouldReturnFalse_WhenNotExists()
        {
            _teacherRepoMock.Setup(r => r.GetById(99)).Returns((Teacher?)null);

            var result = _teacherService.Delete(99);

            result.Should().BeFalse();
        }

        [Fact]
        public void Update_ShouldUpdateTeacher_WhenExists()
        {
            var existing = new Teacher { Id = 1, Name = "Old Name", Email = "old@t.com", Phone = "0100", Specialization = "Math", HireDate = DateTime.Now.AddYears(-1) };
            var updated = new Teacher { Id = 1, Name = "New Name", Email = "new@t.com", Phone = "0200", Specialization = "Science", HireDate = DateTime.Now.AddYears(-2) };

            _teacherRepoMock.Setup(r => r.GetById(1)).Returns(existing);

            var result = _teacherService.Update(updated);

            result.Should().BeTrue();
            existing.Name.Should().Be("New Name");
            existing.Specialization.Should().Be("Science");
        }

        [Fact]
        public void Update_ShouldReturnFalse_WhenNotExists()
        {
            _teacherRepoMock.Setup(r => r.GetById(99)).Returns((Teacher?)null);
            var teacher = new Teacher { Id = 99, Name = "Ahmed" };

            var result = _teacherService.Update(teacher);

            result.Should().BeFalse();
        }
    }
}