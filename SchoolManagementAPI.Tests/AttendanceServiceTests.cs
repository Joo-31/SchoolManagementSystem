using FluentAssertions;
using Moq;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;
using SchoolManagementAPI.Services;
using Xunit;

namespace SchoolManagementAPI.Tests
{
    public class AttendanceServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IAttendanceRepository> _attendanceRepoMock;
        private readonly AttendanceService _attendanceService;

        public AttendanceServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _attendanceRepoMock = new Mock<IAttendanceRepository>();
            _unitOfWorkMock.Setup(u => u.Attendances).Returns(_attendanceRepoMock.Object);
            _attendanceService = new AttendanceService(_unitOfWorkMock.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnAllAttendances()
        {
            var attendances = new List<Attendance>
            {
                new Attendance { Id = 1, StudentId = 1, ClassId = 1, Date = DateTime.Today, IsPresent = true },
                new Attendance { Id = 2, StudentId = 2, ClassId = 1, Date = DateTime.Today, IsPresent = false }
            };
            _attendanceRepoMock.Setup(r => r.GetAll()).Returns(attendances);

            var result = _attendanceService.GetAll();

            result.Should().HaveCount(2);
        }

        [Fact]
        public void GetById_ShouldReturnAttendance_WhenExists()
        {
            var attendance = new Attendance { Id = 1, StudentId = 1, IsPresent = true };
            _attendanceRepoMock.Setup(r => r.GetById(1)).Returns(attendance);

            var result = _attendanceService.GetById(1);

            result.Should().NotBeNull();
            result!.IsPresent.Should().BeTrue();
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenNotExists()
        {
            _attendanceRepoMock.Setup(r => r.GetById(99)).Returns((Attendance?)null);

            var result = _attendanceService.GetById(99);

            result.Should().BeNull();
        }

        [Fact]
        public void Add_ShouldAddAttendanceAndSaveChanges()
        {
            var attendance = new Attendance { Id = 1, StudentId = 1, ClassId = 1, Date = DateTime.Today, IsPresent = true };

            _attendanceService.Add(attendance);

            _attendanceRepoMock.Verify(r => r.Add(attendance), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Delete_ShouldDeleteAttendance_WhenExists()
        {
            var attendance = new Attendance { Id = 1 };
            _attendanceRepoMock.Setup(r => r.GetById(1)).Returns(attendance);

            var result = _attendanceService.Delete(1);

            result.Should().BeTrue();
            _attendanceRepoMock.Verify(r => r.Delete(attendance), Times.Once);
        }

        [Fact]
        public void Delete_ShouldReturnFalse_WhenNotExists()
        {
            _attendanceRepoMock.Setup(r => r.GetById(99)).Returns((Attendance?)null);

            var result = _attendanceService.Delete(99);

            result.Should().BeFalse();
        }

        [Fact]
        public void Update_ShouldUpdateAttendance_WhenExists()
        {
            var existing = new Attendance { Id = 1, StudentId = 1, ClassId = 1, Date = DateTime.Today, IsPresent = true };
            var updated = new Attendance { Id = 1, StudentId = 1, ClassId = 1, Date = DateTime.Today, IsPresent = false };

            _attendanceRepoMock.Setup(r => r.GetById(1)).Returns(existing);

            var result = _attendanceService.Update(updated);

            result.Should().BeTrue();
            existing.IsPresent.Should().BeFalse();
        }

        [Fact]
        public void Update_ShouldReturnFalse_WhenNotExists()
        {
            _attendanceRepoMock.Setup(r => r.GetById(99)).Returns((Attendance?)null);
            var attendance = new Attendance { Id = 99 };

            var result = _attendanceService.Update(attendance);

            result.Should().BeFalse();
        }

        [Fact]
        public void GetPresentCountByDate_ShouldReturnCount()
        {
            _attendanceRepoMock.Setup(r => r.GetPresentCountByDate(DateTime.Today)).Returns(5);

            var result = _attendanceService.GetPresentCountByDate(DateTime.Today);

            result.Should().Be(5);
        }

        [Fact]
        public void GetAbsentCountByDate_ShouldReturnCount()
        {
            _attendanceRepoMock.Setup(r => r.GetAbsentCountByDate(DateTime.Today)).Returns(3);

            var result = _attendanceService.GetAbsentCountByDate(DateTime.Today);

            result.Should().Be(3);
        }
    }
}