using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SchoolManagementAPI.Controllers;
using SchoolManagementAPI.DTOs.Requests;
using SchoolManagementAPI.DTOs.Responses;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Services.Interfaces;
using Xunit;

namespace SchoolManagementAPI.Tests
{
    public class TeachersControllerTests
    {
        private readonly Mock<ITeacherService> _teacherServiceMock;
        private readonly Mock<IClassService> _classServiceMock;
        private readonly Mock<IStudentService> _studentServiceMock;
        private readonly Mock<ICourseService> _courseServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly TeachersController _controller;

        public TeachersControllerTests()
        {
            _teacherServiceMock = new Mock<ITeacherService>();
            _classServiceMock = new Mock<IClassService>();
            _studentServiceMock = new Mock<IStudentService>();
            _courseServiceMock = new Mock<ICourseService>();
            _mapperMock = new Mock<IMapper>();

            _controller = new TeachersController(
                _teacherServiceMock.Object,
                _mapperMock.Object,
                _classServiceMock.Object,
                _studentServiceMock.Object,
                _courseServiceMock.Object
            );
        }

        [Fact]
        public void GetById_ShouldReturnOk_WhenTeacherExists()
        {
            // Arrange
            var teacher = new Teacher { Id = 1, Name = "Ahmed" };
            var teacherDto = new TeacherDto { Id = 1, Name = "Ahmed" };

            _teacherServiceMock.Setup(s => s.GetById(1)).Returns(teacher);
            _mapperMock.Setup(m => m.Map<TeacherDto>(teacher)).Returns(teacherDto);

            // Act
            var result = _controller.GetById(1);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(teacherDto);
        }

        [Fact]
        public void GetById_ShouldReturnNotFound_WhenTeacherDoesNotExist()
        {
            // Arrange
            _teacherServiceMock.Setup(s => s.GetById(99)).Returns((Teacher?)null);

            // Act
            var result = _controller.GetById(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void Delete_ShouldReturnOk_WhenExists()
        {
            // Arrange
            _teacherServiceMock.Setup(s => s.Delete(1)).Returns(true);

            // Act
            var result = _controller.Delete(1);

            // Assert
            result.Should().BeOfType<OkResult>();
        }
    }
}