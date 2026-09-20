using FluentAssertions;
using Moq;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;
using SchoolManagementAPI.Services;
using Xunit;

namespace SchoolManagementAPI.Tests
{
    public class StudentServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly StudentService _studentService;

        public StudentServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _studentService = new StudentService(_unitOfWorkMock.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnAllStudents()
        {
            // Arrange
            var students = new List<Student>
            {
                new Student { Id = 1, FirstName = "Omar", LastName = "Hassan", BirthDate = new DateTime(2010, 5, 15), ClassId = 1 },
                new Student { Id = 2, FirstName = "Sara", LastName = "Ahmed", BirthDate = new DateTime(2011, 8, 20), ClassId = 1 }
            };

            _unitOfWorkMock.Setup(u => u.Students.GetAll()).Returns(students);

            // Act
            var result = _studentService.GetAll();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(s => s.FirstName == "Omar");
        }

        [Fact]
        public void GetById_ShouldReturnStudent_WhenExists()
        {
            // Arrange
            var student = new Student { Id = 1, FirstName = "Omar", LastName = "Hassan", BirthDate = new DateTime(2010, 5, 15), ClassId = 1 };
            _unitOfWorkMock.Setup(u => u.Students.GetById(1)).Returns(student);

            // Act
            var result = _studentService.GetById(1);

            // Assert
            result.Should().NotBeNull();
            result!.FirstName.Should().Be("Omar");
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Students.GetById(99)).Returns((Student?)null);

            // Act
            var result = _studentService.GetById(99);

            // Assert
            result.Should().BeNull();
        }
    }
}