using FluentAssertions;
using Moq;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;
using SchoolManagementAPI.Services;
using Xunit;

namespace SchoolManagementAPI.Tests
{
    public class CourseServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICourseRepository> _courseRepoMock;
        private readonly CourseService _courseService;

        public CourseServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _courseRepoMock = new Mock<ICourseRepository>();
            _unitOfWorkMock.Setup(u => u.Courses).Returns(_courseRepoMock.Object);
            _courseService = new CourseService(_unitOfWorkMock.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnAllCourses()
        {
            var courses = new List<Course>
            {
                new Course { Id = 1, Name = "Math", Code = "MATH101", Credits = 3, GradeId = 1, TeacherId = 1 },
                new Course { Id = 2, Name = "English", Code = "ENG101", Credits = 2, GradeId = 1, TeacherId = 2 }
            };
            _courseRepoMock.Setup(r => r.GetAll()).Returns(courses);

            var result = _courseService.GetAll();

            result.Should().HaveCount(2);
        }

        [Fact]
        public void GetById_ShouldReturnCourse_WhenExists()
        {
            var course = new Course { Id = 1, Name = "Math", Code = "MATH101", Credits = 3 };
            _courseRepoMock.Setup(r => r.GetById(1)).Returns(course);

            var result = _courseService.GetById(1);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Math");
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenNotExists()
        {
            _courseRepoMock.Setup(r => r.GetById(99)).Returns((Course?)null);

            var result = _courseService.GetById(99);

            result.Should().BeNull();
        }

        [Fact]
        public void Add_ShouldAddCourseAndSaveChanges()
        {
            var course = new Course { Id = 1, Name = "Math", Code = "MATH101", Credits = 3 };

            _courseService.Add(course);

            _courseRepoMock.Verify(r => r.Add(course), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Delete_ShouldDeleteCourse_WhenExists()
        {
            var course = new Course { Id = 1, Name = "Math" };
            _courseRepoMock.Setup(r => r.GetById(1)).Returns(course);

            var result = _courseService.Delete(1);

            result.Should().BeTrue();
            _courseRepoMock.Verify(r => r.Delete(course), Times.Once);
        }

        [Fact]
        public void Delete_ShouldReturnFalse_WhenNotExists()
        {
            _courseRepoMock.Setup(r => r.GetById(99)).Returns((Course?)null);

            var result = _courseService.Delete(99);

            result.Should().BeFalse();
        }

        [Fact]
        public void Update_ShouldUpdateCourse_WhenExists()
        {
            var existing = new Course { Id = 1, Name = "Old Math", Code = "MATH101", Credits = 3, GradeId = 1, TeacherId = 1 };
            var updated = new Course { Id = 1, Name = "New Math", Code = "MATH102", Credits = 4, GradeId = 2, TeacherId = 2 };

            _courseRepoMock.Setup(r => r.GetById(1)).Returns(existing);

            var result = _courseService.Update(updated);

            result.Should().BeTrue();
            existing.Name.Should().Be("New Math");
            existing.Credits.Should().Be(4);
        }

        [Fact]
        public void Update_ShouldReturnFalse_WhenNotExists()
        {
            _courseRepoMock.Setup(r => r.GetById(99)).Returns((Course?)null);
            var course = new Course { Id = 99, Name = "Math" };

            var result = _courseService.Update(course);

            result.Should().BeFalse();
        }
    }
}