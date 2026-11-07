using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementAPI.DTOs.Requests;
using SchoolManagementAPI.Services.Interfaces;
using System;
using System.Security.Claims;

namespace SchoolManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HomeworkController : ControllerBase
    {
        private readonly IHomeworkService _homeworkService;

        public HomeworkController(IHomeworkService homeworkService)
        {
            _homeworkService = homeworkService;
        }

        // POST: api/homework (Teacher only)
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public IActionResult Create([FromBody] CreateHomeworkDto dto)
        {
            var teacherIdStr = User.FindFirst("TeacherId")?.Value;
            if (string.IsNullOrEmpty(teacherIdStr)) return Unauthorized();

            var teacherId = int.Parse(teacherIdStr);

            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Link))
                return BadRequest("Title and Link are required.");

            _homeworkService.Create(dto, teacherId);
            return Ok(new { message = "Homework added successfully!" });
        }

        // GET: api/homework/my-homework (Teacher only)
        [HttpGet("my-homework")]
        [Authorize(Roles = "Teacher")]
        public IActionResult GetTeacherHomework()
        {
            var teacherIdStr = User.FindFirst("TeacherId")?.Value;
            if (string.IsNullOrEmpty(teacherIdStr)) return Unauthorized();

            var teacherId = int.Parse(teacherIdStr);
            var result = _homeworkService.GetByTeacherId(teacherId);
            return Ok(result);
        }

        // GET: api/homework/student (Student only)
        [HttpGet("student")]
        [Authorize(Roles = "Student")]
        public IActionResult GetStudentHomework()
        {
            var studentIdStr = User.FindFirst("StudentId")?.Value;
            if (string.IsNullOrEmpty(studentIdStr)) return Unauthorized();

            var studentId = int.Parse(studentIdStr);
            var result = _homeworkService.GetByStudentId(studentId);
            return Ok(result);
        }

        // DELETE: api/homework/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Delete(int id)
        {
            try
            {
                int? teacherId = null;
                if (User.IsInRole("Teacher"))
                {
                    var teacherIdStr = User.FindFirst("TeacherId")?.Value;
                    if (!string.IsNullOrEmpty(teacherIdStr))
                        teacherId = int.Parse(teacherIdStr);
                }

                _homeworkService.Delete(id, teacherId);
                return Ok(new { message = "Deleted successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }
    }
}