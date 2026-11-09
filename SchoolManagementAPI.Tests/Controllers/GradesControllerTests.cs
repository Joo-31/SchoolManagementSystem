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
    public class GradesControllerTests
    {
        private readonly Mock<IGradeService> _gradeServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GradesController _controller;

        public GradesControllerTests()
        {
            _gradeServiceMock = new Mock<IGradeService>();
            _mapperMock = new Mock<IMapper>();
            _controller = new GradesController(_gradeServiceMock.Object, _mapperMock.Object);
        }

        // ✅ اختبار 1: جلب كل الدرجات (لما يكون فيه داتا)
        [Fact]
        public void GetAll_ShouldReturnOkWithData()
        {
            // Arrange
            var grades = new List<Grade> { new Grade { Id = 1, Name = "Primary", Level = 1 } };
            var gradesDto = new List<GradeDto> { new GradeDto { Id = 1, Name = "Primary", Level = 1 } };

            _gradeServiceMock.Setup(s => s.GetAll()).Returns(grades);
            _mapperMock.Setup(m => m.Map<List<GradeDto>>(grades)).Returns(gradesDto);

            // Act
            var result = _controller.GetAll();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(gradesDto);
        }

        // ✅ اختبار 2: جلب Grade بالـ ID (لما يكون موجود)
        [Fact]
        public void GetById_ShouldReturnOk_WhenGradeExists()
        {
            // Arrange
            var grade = new Grade { Id = 1, Name = "Primary" };
            var gradeDto = new GradeDto { Id = 1, Name = "Primary" };

            _gradeServiceMock.Setup(s => s.GetById(1)).Returns(grade);
            _mapperMock.Setup(m => m.Map<GradeDto>(grade)).Returns(gradeDto);

            // Act
            var result = _controller.GetById(1);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(gradeDto);
        }

        // ✅ اختبار 3: جلب Grade بالـ ID (لما مش موجود)
        [Fact]
        public void GetById_ShouldReturnNotFound_WhenGradeDoesNotExist()
        {
            // Arrange
            _gradeServiceMock.Setup(s => s.GetById(99)).Returns((Grade?)null);

            // Act
            var result = _controller.GetById(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        // ✅ اختبار 4: إضافة Grade جديدة
        [Fact]
        public void Add_ShouldReturnOk_WhenValid()
        {
            // Arrange
            var dto = new CreateGradeDto { Name = "Secondary", Level = 3 };
            var grade = new Grade { Id = 1, Name = "Secondary", Level = 3 };
            var gradeDto = new GradeDto { Id = 1, Name = "Secondary", Level = 3 };

            _mapperMock.Setup(m => m.Map<Grade>(dto)).Returns(grade);
            _mapperMock.Setup(m => m.Map<GradeDto>(grade)).Returns(gradeDto);

            // Act
            var result = _controller.Add(dto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(gradeDto);
            _gradeServiceMock.Verify(s => s.Add(grade), Times.Once);
        }

        // ✅ اختبار 5: حذف Grade (لما تكون موجودة)
        [Fact]
        public void Delete_ShouldReturnOk_WhenExists()
        {
            // Arrange
            _gradeServiceMock.Setup(s => s.Delete(1)).Returns(true);

            // Act
            var result = _controller.Delete(1);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        // ✅ اختبار 6: حذف Grade (لما تكون مش موجودة)
        [Fact]
        public void Delete_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            _gradeServiceMock.Setup(s => s.Delete(99)).Returns(false);

            // Act
            var result = _controller.Delete(99);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}