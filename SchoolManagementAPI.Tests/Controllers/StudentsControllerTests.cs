using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SchoolManagementAPI.Controllers;
using SchoolManagementAPI.DTOs.Requests;
using SchoolManagementAPI.DTOs.Responses;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Services.Interfaces;
using System.Security.Claims;
using Xunit;

namespace SchoolManagementAPI.Tests
{
    public class StudentsControllerTests
    {
        private readonly Mock<IStudentService> _studentServiceMock;
        private readonly Mock<IAttendanceService> _attendanceServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly StudentsController _controller;

        public StudentsControllerTests()
        {
            _studentServiceMock = new Mock<IStudentService>();
            _attendanceServiceMock = new Mock<IAttendanceService>();
            _mapperMock = new Mock<IMapper>();
            _controller = new StudentsController(_studentServiceMock.Object, _attendanceServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void GetById_ShouldReturnOk_WhenStudentExists()
        {
            // Arrange
            var student = new Student { Id = 1, FirstName = "Omar", LastName = "Hassan" };
            var studentDto = new StudentDto { Id = 1, FullName = "Omar Hassan" };

            _studentServiceMock.Setup(s => s.GetById(1)).Returns(student);
            _mapperMock.Setup(m => m.Map<StudentDto>(student)).Returns(studentDto);

            // Act
            var result = _controller.GetById(1);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(studentDto);
        }

        [Fact]
        public void GetById_ShouldReturnNotFound_WhenStudentDoesNotExist()
        {
            // Arrange
            _studentServiceMock.Setup(s => s.GetById(99)).Returns((Student?)null);

            // Act
            var result = _controller.GetById(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void Add_ShouldReturnOk_WhenValid()
        {
            // Arrange
            var dto = new CreateStudentDto { FirstName = "Ali", LastName = "Ahmed", Username = "user1", Password = "Password@123" };
            var student = new Student { Id = 1, FirstName = "Ali", LastName = "Ahmed" };
            var studentDto = new StudentDto { Id = 1, FullName = "Ali Ahmed" };

            _mapperMock.Setup(m => m.Map<Student>(dto)).Returns(student);
            _mapperMock.Setup(m => m.Map<StudentDto>(student)).Returns(studentDto);

            // Act
            var result = _controller.Add(dto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(studentDto);
            _studentServiceMock.Verify(s => s.Add(student, "user1", "Password@123"), Times.Once);
        }

        [Fact]
        public void Delete_ShouldReturnOk_WhenExists()
        {
            // Arrange
            _studentServiceMock.Setup(s => s.Delete(1)).Returns(true);

            // Act
            var result = _controller.Delete(1);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public void Delete_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            _studentServiceMock.Setup(s => s.Delete(99)).Returns(false);

            // Act
            var result = _controller.Delete(99);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}