using FluentAssertions;
using Moq;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;
using SchoolManagementAPI.Services;
using Xunit;

namespace SchoolManagementAPI.Tests
{
    public class ClassServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IClassRepository> _classRepoMock;
        private readonly ClassService _classService;

        public ClassServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _classRepoMock = new Mock<IClassRepository>();
            _unitOfWorkMock.Setup(u => u.Classes).Returns(_classRepoMock.Object);
            _classService = new ClassService(_unitOfWorkMock.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnAllClasses()
        {
            var classes = new List<Class>
            {
                new Class { Id = 1, Name = "3A", GradeId = 1, ClassTeacherId = 1 },
                new Class { Id = 2, Name = "3B", GradeId = 1, ClassTeacherId = 2 }
            };
            _classRepoMock.Setup(r => r.GetAll()).Returns(classes);

            var result = _classService.GetAll();

            result.Should().HaveCount(2);
        }

        [Fact]
        public void GetById_ShouldReturnClass_WhenExists()
        {
            var classObj = new Class { Id = 1, Name = "3A" };
            _classRepoMock.Setup(r => r.GetById(1)).Returns(classObj);

            var result = _classService.GetById(1);

            result.Should().NotBeNull();
            result!.Name.Should().Be("3A");
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenNotExists()
        {
            _classRepoMock.Setup(r => r.GetById(99)).Returns((Class?)null);

            var result = _classService.GetById(99);

            result.Should().BeNull();
        }

        [Fact]
        public void Add_ShouldAddClassAndSaveChanges()
        {
            var classObj = new Class { Id = 1, Name = "3A", GradeId = 1, ClassTeacherId = 1 };

            _classService.Add(classObj);

            _classRepoMock.Verify(r => r.Add(classObj), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Delete_ShouldDeleteClass_WhenExists()
        {
            var classObj = new Class { Id = 1, Name = "3A" };
            _classRepoMock.Setup(r => r.GetById(1)).Returns(classObj);

            var result = _classService.Delete(1);

            result.Should().BeTrue();
            _classRepoMock.Verify(r => r.Delete(classObj), Times.Once);
        }

        [Fact]
        public void Delete_ShouldReturnFalse_WhenNotExists()
        {
            _classRepoMock.Setup(r => r.GetById(99)).Returns((Class?)null);

            var result = _classService.Delete(99);

            result.Should().BeFalse();
        }

        [Fact]
        public void Update_ShouldUpdateClass_WhenExists()
        {
            var existing = new Class { Id = 1, Name = "Old Class", GradeId = 1, ClassTeacherId = 1 };
            var updated = new Class { Id = 1, Name = "New Class", GradeId = 2, ClassTeacherId = 2 };

            _classRepoMock.Setup(r => r.GetById(1)).Returns(existing);

            var result = _classService.Update(updated);

            result.Should().BeTrue();
            existing.Name.Should().Be("New Class");
            existing.GradeId.Should().Be(2);
        }

        [Fact]
        public void Update_ShouldReturnFalse_WhenNotExists()
        {
            _classRepoMock.Setup(r => r.GetById(99)).Returns((Class?)null);
            var classObj = new Class { Id = 99, Name = "Test" };

            var result = _classService.Update(classObj);

            result.Should().BeFalse();
        }
    }
}