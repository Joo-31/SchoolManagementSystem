using FluentAssertions;
using Moq;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;
using SchoolManagementAPI.Services;
using Xunit;

namespace SchoolManagementAPI.Tests
{
    public class GradeServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IGradeRepository> _gradeRepoMock;
        private readonly GradeService _gradeService;

        public GradeServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _gradeRepoMock = new Mock<IGradeRepository>();
            _unitOfWorkMock.Setup(u => u.Grades).Returns(_gradeRepoMock.Object);
            _gradeService = new GradeService(_unitOfWorkMock.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnAllGrades()
        {
            var grades = new List<Grade>
            {
                new Grade { Id = 1, Name = "Primary", Level = 1 },
                new Grade { Id = 2, Name = "Secondary", Level = 2 }
            };
            _gradeRepoMock.Setup(r => r.GetAll()).Returns(grades);

            var result = _gradeService.GetAll();

            result.Should().HaveCount(2);
        }

        [Fact]
        public void GetById_ShouldReturnGrade_WhenExists()
        {
            var grade = new Grade { Id = 1, Name = "Primary", Level = 1 };
            _gradeRepoMock.Setup(r => r.GetById(1)).Returns(grade);

            var result = _gradeService.GetById(1);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Primary");
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenNotExists()
        {
            _gradeRepoMock.Setup(r => r.GetById(99)).Returns((Grade?)null);

            var result = _gradeService.GetById(99);

            result.Should().BeNull();
        }

        [Fact]
        public void Add_ShouldAddGradeAndSaveChanges()
        {
            var grade = new Grade { Id = 1, Name = "Primary", Level = 1 };

            _gradeService.Add(grade);

            _gradeRepoMock.Verify(r => r.Add(grade), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Delete_ShouldDeleteGrade_WhenExists()
        {
            var grade = new Grade { Id = 1, Name = "Primary" };
            _gradeRepoMock.Setup(r => r.GetById(1)).Returns(grade);

            var result = _gradeService.Delete(1);

            result.Should().BeTrue();
            _gradeRepoMock.Verify(r => r.Delete(grade), Times.Once);
        }

        [Fact]
        public void Delete_ShouldReturnFalse_WhenNotExists()
        {
            _gradeRepoMock.Setup(r => r.GetById(99)).Returns((Grade?)null);

            var result = _gradeService.Delete(99);

            result.Should().BeFalse();
        }

        [Fact]
        public void Update_ShouldUpdateGrade_WhenExists()
        {
            var existing = new Grade { Id = 1, Name = "Old Grade", Level = 1 };
            var updated = new Grade { Id = 1, Name = "New Grade", Level = 2 };

            _gradeRepoMock.Setup(r => r.GetById(1)).Returns(existing);

            var result = _gradeService.Update(updated);

            result.Should().BeTrue();
            existing.Name.Should().Be("New Grade");
            existing.Level.Should().Be(2);
        }

        [Fact]
        public void Update_ShouldReturnFalse_WhenNotExists()
        {
            _gradeRepoMock.Setup(r => r.GetById(99)).Returns((Grade?)null);
            var grade = new Grade { Id = 99, Name = "Test" };

            var result = _gradeService.Update(grade);

            result.Should().BeFalse();
        }
    }
}