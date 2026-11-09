using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SchoolManagementAPI.DTOs.Requests;
using SchoolManagementAPI.DTOs.Responses;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Services.Interfaces;

namespace SchoolManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MarksController : ControllerBase
    {
        private readonly IMarkService _markService;
        private readonly ICourseService _courseService;
        private readonly IClassService _classService;
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;

        public MarksController(IMarkService markService, ICourseService courseService, IClassService classService, IStudentService studentService, IMapper mapper)
        {
            _markService = markService;
            _courseService = courseService;
            _classService = classService;
            _studentService = studentService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetAll()
        {
            var marks = _markService.GetAll();
            return Ok(_mapper.Map<List<MarkDto>>(marks));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetById(int id)
        {
            var mark = _markService.GetById(id);
            if (mark == null) return NotFound();
            return Ok(_mapper.Map<MarkDto>(mark));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Add([FromBody] CreateMarkDto dto)
        {
            try
            {
                int teacherId;
                if (User.IsInRole("Teacher"))
                {
                    var teacherIdStr = User.FindFirst("TeacherId")?.Value;
                    if (string.IsNullOrEmpty(teacherIdStr)) return Unauthorized("Teacher ID not found");
                    teacherId = int.Parse(teacherIdStr);
                }
                else
                {
                    if (dto.TeacherId <= 0) return BadRequest("TeacherId is required");
                    teacherId = dto.TeacherId;
                }

                var course = _courseService.GetById(dto.CourseId);
                if (course == null) return NotFound("Course not found");
                if (course.TeacherId != teacherId) return StatusCode(403, new { message = "You are not the teacher of this course" });

                var existingMarks = _markService.GetByStudentId(dto.StudentId);
                if (existingMarks.Any(m => m.CourseId == dto.CourseId)) return BadRequest("Student already has a mark for this course.");

                var mark = new Mark
                {
                    StudentId = dto.StudentId,
                    CourseId = dto.CourseId,
                    TeacherId = teacherId,
                    Score = dto.Score,
                    Date = dto.Date,
                    Notes = dto.Notes
                };

                _markService.Add(mark);
                var savedMark = _markService.GetById(mark.Id);
                return Ok(_mapper.Map<MarkDto>(savedMark));
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Update(int id, [FromBody] UpdateMarkDto dto)
        {
            var existing = _markService.GetById(id);
            if (existing == null) return NotFound();
            _mapper.Map(dto, existing);
            _markService.Update(existing);
            var savedMark = _markService.GetById(id);
            return Ok(_mapper.Map<MarkDto>(savedMark));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Delete(int id)
        {
            var mark = _markService.GetById(id);
            if (mark == null) return NotFound();
            if (User.IsInRole("Teacher"))
            {
                var teacherIdStr = User.FindFirst("TeacherId")?.Value;
                if (string.IsNullOrEmpty(teacherIdStr)) return Unauthorized();
                if (mark.TeacherId != int.Parse(teacherIdStr)) return StatusCode(403, new { message = "You can only delete your own marks" });
            }
            _markService.Delete(id);
            return Ok(new { message = "Mark deleted successfully" });
        }

        [HttpGet("me")]
        [Authorize(Roles = "Student")]
        public IActionResult GetMyMarks()
        {
            var studentIdStr = User.FindFirst("StudentId")?.Value;
            if (string.IsNullOrEmpty(studentIdStr)) return Unauthorized("Student ID not found");
            var marks = _markService.GetByStudentId(int.Parse(studentIdStr));
            return Ok(_mapper.Map<List<MarkDto>>(marks));
        }

        [HttpGet("my-marks")]
        [Authorize(Roles = "Teacher")]
        public IActionResult GetMarksByTeacher()
        {
            var teacherIdStr = User.FindFirst("TeacherId")?.Value;
            if (string.IsNullOrEmpty(teacherIdStr)) return Unauthorized();
            var marks = _markService.GetByTeacherId(int.Parse(teacherIdStr));
            return Ok(_mapper.Map<List<MarkDto>>(marks));
        }

        [HttpGet("my-courses")]
        [Authorize(Roles = "Teacher")]
        public IActionResult GetMyCourses()
        {
            var teacherIdStr = User.FindFirst("TeacherId")?.Value;
            if (string.IsNullOrEmpty(teacherIdStr)) return Unauthorized();
            var courses = _courseService.GetAll().Where(c => c.TeacherId == int.Parse(teacherIdStr)).ToList();
            return Ok(_mapper.Map<List<CourseDto>>(courses));
        }

        [HttpGet("paged")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetPaged([FromQuery] PaginationParams paginationParams)
        {
            int? teacherId = null;
            if (User.IsInRole("Teacher"))
            {
                var teacherIdStr = User.FindFirst("TeacherId")?.Value;
                if (!string.IsNullOrEmpty(teacherIdStr))
                    teacherId = int.Parse(teacherIdStr);
            }

            var pagedResult = _markService.GetPaged(paginationParams.PageNumber, paginationParams.PageSize, teacherId);
            var marksDto = _mapper.Map<List<MarkDto>>(pagedResult.Data);

            var result = new PagedResult<MarkDto>
            {
                Data = marksDto,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize,
                TotalCount = pagedResult.TotalCount,
                TotalPages = pagedResult.TotalPages
            };

            return Ok(result);
        }

        // ✅ GetMyStudents (تم تحسين الأداء)
        [HttpGet("my-students")]
        [Authorize(Roles = "Teacher")]
        public IActionResult GetMyStudents()
        {
            var teacherIdStr = User.FindFirst("TeacherId")?.Value;
            if (string.IsNullOrEmpty(teacherIdStr)) return Unauthorized();
            var teacherId = int.Parse(teacherIdStr);

            // ✅ نجيب كل الطلاب اللي في فصول المدرس مرة واحدة
            var classIds = _classService.GetClassesByTeacherId(teacherId).Select(c => c.Id).ToList();

            var students = _studentService.GetAll()
                .Where(s => classIds.Contains(s.ClassId))
                .ToList();

            return Ok(_mapper.Map<List<StudentDto>>(students));
        }
    }
}