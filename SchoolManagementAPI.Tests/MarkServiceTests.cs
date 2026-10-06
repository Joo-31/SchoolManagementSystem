using FluentAssertions;
using Moq;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;
using SchoolManagementAPI.Services;
using Xunit;

namespace SchoolManagementAPI.Tests
{
    public class MarkServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMarkRepository> _markRepoMock;
        private readonly MarkService _markService;

        public MarkServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _markRepoMock = new Mock<IMarkRepository>();

            // ✅ Setup Marks Property
            _unitOfWorkMock.Setup(u => u.Marks).Returns(_markRepoMock.Object);

            _markService = new MarkService(_unitOfWorkMock.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnAllMarks()
        {
            // Arrange
            var marks = new List<Mark>
            {
                new Mark { Id = 1, StudentId = 1, CourseId = 1, TeacherId = 1, Score = 95 },
                new Mark { Id = 2, StudentId = 1, CourseId = 2, TeacherId = 2, Score = 85 }
            };

            _markRepoMock.Setup(r => r.GetAll()).Returns(marks);

            // Act
            var result = _markService.GetAll();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(m => m.Score == 95);
        }

        [Fact]
        public void GetById_ShouldReturnMark_WhenExists()
        {
            // Arrange
            var mark = new Mark { Id = 1, StudentId = 1, CourseId = 1, TeacherId = 1, Score = 95 };
            _markRepoMock.Setup(r => r.GetById(1)).Returns(mark);

            // Act
            var result = _markService.GetById(1);

            // Assert
            result.Should().NotBeNull();
            result!.Score.Should().Be(95);
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            _markRepoMock.Setup(r => r.GetById(99)).Returns((Mark?)null);

            // Act
            var result = _markService.GetById(99);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Add_ShouldAddMarkAndSaveChanges()
        {
            // Arrange
            var mark = new Mark { Id = 1, StudentId = 1, CourseId = 1, TeacherId = 1, Score = 95 };

            // Act
            _markService.Add(mark);

            // Assert
            _markRepoMock.Verify(r => r.Add(mark), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Delete_ShouldDeleteMark_WhenExists()
        {
            // Arrange
            var mark = new Mark { Id = 1, StudentId = 1, CourseId = 1, TeacherId = 1, Score = 95 };
            _markRepoMock.Setup(r => r.GetById(1)).Returns(mark);

            // Act
            var result = _markService.Delete(1);

            // Assert
            result.Should().BeTrue();
            _markRepoMock.Verify(r => r.Delete(mark), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Delete_ShouldReturnFalse_WhenNotExists()
        {
            // Arrange
            _markRepoMock.Setup(r => r.GetById(99)).Returns((Mark?)null);

            // Act
            var result = _markService.Delete(99);

            // Assert
            result.Should().BeFalse();
            _markRepoMock.Verify(r => r.Delete(It.IsAny<Mark>()), Times.Never);
        }

        [Fact]
        public void GetByStudentId_ShouldReturnStudentMarks()
        {
            // Arrange
            var marks = new List<Mark>
            {
                new Mark { Id = 1, StudentId = 1, CourseId = 1, Score = 95 },
                new Mark { Id = 2, StudentId = 1, CourseId = 2, Score = 85 }
            };
            _markRepoMock.Setup(r => r.GetByStudentId(1)).Returns(marks);

            // Act
            var result = _markService.GetByStudentId(1);

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(m => m.StudentId.Should().Be(1));
        }

        [Fact]
        public void Update_ShouldUpdateMark_WhenExists()
        {
            // Arrange
            var existingMark = new Mark { Id = 1, StudentId = 1, CourseId = 1, TeacherId = 1, Score = 95 };
            var updatedMark = new Mark { Id = 1, StudentId = 1, CourseId = 1, TeacherId = 1, Score = 100 };

            _markRepoMock.Setup(r => r.GetById(1)).Returns(existingMark);

            // Act
            var result = _markService.Update(updatedMark);

            // Assert
            result.Should().BeTrue();
            existingMark.Score.Should().Be(100);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Update_ShouldReturnFalse_WhenNotExists()
        {
            // Arrange
            _markRepoMock.Setup(r => r.GetById(99)).Returns((Mark?)null);
            var mark = new Mark { Id = 99, Score = 95 };

            // Act
            var result = _markService.Update(mark);

            // Assert
            result.Should().BeFalse();
        }
    }
}