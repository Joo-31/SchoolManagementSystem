using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementAPI.DTOs.Requests;
using SchoolManagementAPI.DTOs.Responses;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Services.Interfaces;
using System.Security.Claims;

namespace SchoolManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _teacherService;
        private readonly IMapper _mapper;
        private readonly IClassService _classService;
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;

        public TeachersController(ITeacherService teacherService, IMapper mapper, IClassService classService, IStudentService studentService, ICourseService courseService)
        {
            _teacherService = teacherService;
            _mapper = mapper;
            _classService = classService;
            _studentService = studentService;
            _courseService = courseService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            var teachers = _teacherService.GetAll();
            return Ok(_mapper.Map<List<TeacherDto>>(teachers));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetById(int id)
        {
            var teacher = _teacherService.GetById(id);
            if (teacher == null) return NotFound();
            return Ok(_mapper.Map<TeacherDto>(teacher));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Add([FromBody] CreateTeacherDto dto)
        {
            try
            {
                var teacher = _mapper.Map<Teacher>(dto);
                _teacherService.Add(teacher, dto.Username, dto.Password);
                return Ok(_mapper.Map<TeacherDto>(teacher));
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UpdateTeacherDto dto)
        {
            var existing = _teacherService.GetById(id);
            if (existing == null) return NotFound("Teacher not found");
            _mapper.Map(dto, existing);
            _teacherService.Update(existing);
            return Ok(_mapper.Map<TeacherDto>(existing));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var deleted = _teacherService.Delete(id);
            if (!deleted) return NotFound("Teacher not found");
            return Ok();
        }

        [HttpGet("paged")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetPaged([FromQuery] PaginationParams paginationParams)
        {
            var pagedResult = _teacherService.GetPaged(paginationParams.PageNumber, paginationParams.PageSize);
            var teachersDto = _mapper.Map<List<TeacherDto>>(pagedResult.Data);
            var result = new PagedResult<TeacherDto>
            {
                Data = teachersDto,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize,
                TotalCount = pagedResult.TotalCount,
                TotalPages = pagedResult.TotalPages
            };
            return Ok(result);
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public IActionResult Search([FromQuery] string? keyword)
        {
            var teachers = _teacherService.Search(keyword);
            return Ok(_mapper.Map<List<TeacherDto>>(teachers));
        }

        [HttpGet("me")]
        [Authorize(Roles = "Teacher")]
        public IActionResult GetMe()
        {
            var teacherIdStr = User.FindFirst("TeacherId")?.Value;
            if (string.IsNullOrEmpty(teacherIdStr)) return Unauthorized("Teacher ID not found");
            var teacherId = int.Parse(teacherIdStr);
            var teacher = _teacherService.GetById(teacherId);
            if (teacher == null) return NotFound();
            return Ok(_mapper.Map<TeacherDto>(teacher));
        }

        [HttpGet("me/classes")]
        [Authorize(Roles = "Teacher")]
        public IActionResult GetMyClasses()
        {
            var teacherIdStr = User.FindFirst("TeacherId")?.Value;
            if (string.IsNullOrEmpty(teacherIdStr)) return Unauthorized("Teacher ID not found");
            var teacherId = int.Parse(teacherIdStr);
            var classes = _classService.GetClassesByTeacherId(teacherId);
            return Ok(_mapper.Map<List<ClassDto>>(classes));
        }

        [HttpGet("me/classes/{classId}/students")]
        [Authorize(Roles = "Teacher")]
        public IActionResult GetStudentsByClass(int classId)
        {
            var teacherIdStr = User.FindFirst("TeacherId")?.Value;
            if (string.IsNullOrEmpty(teacherIdStr)) return Unauthorized("Teacher ID not found");
            var teacherId = int.Parse(teacherIdStr);

            var classObj = _classService.GetById(classId);
            if (classObj == null) return NotFound("Class not found");

            // ✅ بنتأكد إنه رائد الفصل، أو بيدرس كورس في المرحلة بتاعت الفصل ده
            var isClassTeacher = classObj.ClassTeacherId == teacherId;
            var teachesInClass = _courseService.GetAll().Any(c => c.TeacherId == teacherId && c.GradeId == classObj.GradeId);

            if (!isClassTeacher && !teachesInClass)
                return StatusCode(403, new { message = "You are not assigned to this class or its courses." });

            var students = _studentService.GetStudentsByClassId(classId);
            return Ok(_mapper.Map<List<StudentDto>>(students));
        }

        [HttpGet("count")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetCount()
        {
            return Ok(new { Count = _teacherService.GetAll().Count });
        }

        [HttpGet("statistics/count-by-specialization")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetCountBySpecialization()
        {
            var result = _teacherService.GetAll()
                .GroupBy(t => t.Specialization)
                .ToDictionary(g => g.Key, g => g.Count());
            return Ok(result);
        }
    }
}