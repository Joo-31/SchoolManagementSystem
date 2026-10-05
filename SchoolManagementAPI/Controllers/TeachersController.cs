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
    [Authorize]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _teacherService;
        private readonly IMapper _mapper;
        private readonly IClassService _classService;
        private readonly IStudentService _studentService;

        public TeachersController(ITeacherService teacherService, IMapper mapper, IClassService classService, IStudentService studentService)
        {
            _teacherService = teacherService;
            _mapper = mapper;
            _classService = classService;
            _studentService = studentService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            var teachers = _teacherService.GetAll();
            return Ok(_mapper.Map<List<TeacherDto>>(teachers));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var teacher = _teacherService.GetById(id);
            if (teacher == null)
                return NotFound();
            return Ok(_mapper.Map<TeacherDto>(teacher));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Add([FromBody] CreateTeacherDto dto)
        {
            try
            {
                var teacher = _mapper.Map<Teacher>(dto);
                _teacherService.Add(teacher);
                return Ok(_mapper.Map<TeacherDto>(teacher));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UpdateTeacherDto dto)
        {
            var existing = _teacherService.GetById(id);
            if (existing == null)
                return NotFound("Teacher not found");

            _mapper.Map(dto, existing);

            var updated = _teacherService.Update(existing);
            if (!updated)
                return NotFound("No changes were made");

            return Ok(_mapper.Map<TeacherDto>(existing));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var deleted = _teacherService.Delete(id);
            if (!deleted)
                return NotFound("Teacher not found");
            return Ok();
        }
        // GET: api/teachers/paged?pageNumber=1&pageSize=10
        [HttpGet("paged")]
        [AllowAnonymous]
        public IActionResult GetPaged([FromQuery] PaginationParams paginationParams)
        {
            var pagedResult = _teacherService.GetPaged(
                paginationParams.PageNumber,
                paginationParams.PageSize
            );

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
        // GET: api/teachers/search?keyword=ahmed
        [HttpGet("search")]
        [AllowAnonymous]
        public IActionResult Search([FromQuery] string? keyword)
        {
            var teachers = _teacherService.Search(keyword);
            return Ok(_mapper.Map<List<TeacherDto>>(teachers));
        }

        // GET: api/teachers/specialization/Math
        [HttpGet("specialization/{specialization}")]
        [AllowAnonymous]
        public IActionResult GetBySpecialization(string specialization)
        {
            var teachers = _teacherService.GetTeachersBySpecialization(specialization);
            return Ok(_mapper.Map<List<TeacherDto>>(teachers));
        }

        // GET: api/teachers/ordered-by-experience
        [HttpGet("ordered-by-experience")]
        [AllowAnonymous]
        public IActionResult GetOrderedByExperience()
        {
            var teachers = _teacherService.GetTeachersOrderedByExperience();
            return Ok(_mapper.Map<List<TeacherDto>>(teachers));
        }

        // GET: api/teachers/experience-more-than/5
        [HttpGet("experience-more-than/{years}")]
        [AllowAnonymous]
        public IActionResult GetWithExperienceMoreThan(int years)
        {
            var teachers = _teacherService.GetTeachersWithExperienceMoreThan(years);
            return Ok(_mapper.Map<List<TeacherDto>>(teachers));
        }

        // GET: api/teachers/statistics/count-by-specialization
        [HttpGet("statistics/count-by-specialization")]
        [AllowAnonymous]
        public IActionResult GetCountBySpecialization()
        {
            return Ok(_teacherService.GetTeacherCountBySpecialization());
        }

        // GET: api/teachers/statistics/average-experience
        [HttpGet("statistics/average-experience")]
        [AllowAnonymous]
        public IActionResult GetAverageExperience()
        {
            return Ok(new { AverageExperience = _teacherService.GetAverageExperience() });
        }

        // GET: api/teachers/statistics/newest
        [HttpGet("statistics/newest")]
        [AllowAnonymous]
        public IActionResult GetNewest()
        {
            var teacher = _teacherService.GetNewestTeacher();
            if (teacher == null) return NotFound();
            return Ok(_mapper.Map<TeacherDto>(teacher));
        }

        // GET: api/teachers/statistics/oldest
        [HttpGet("statistics/oldest")]
        [AllowAnonymous]
        public IActionResult GetOldest()
        {
            var teacher = _teacherService.GetOldestTeacher();
            if (teacher == null) return NotFound();
            return Ok(_mapper.Map<TeacherDto>(teacher));
        }

        // GET: api/teachers/me
        [HttpGet("me")]
        [Authorize(Roles = "Teacher")]
        public IActionResult GetMe()
        {
            var teacherIdStr = User.FindFirst("TeacherId")?.Value;
            if (string.IsNullOrEmpty(teacherIdStr))
                return Unauthorized("Teacher ID not found");

            var teacherId = int.Parse(teacherIdStr);
            var teacher = _teacherService.GetById(teacherId);

            if (teacher == null)
                return NotFound();

            return Ok(_mapper.Map<TeacherDto>(teacher));
        }

        // GET: api/teachers/me/classes
        [HttpGet("me/classes")]
        [Authorize(Roles = "Teacher")]
        public IActionResult GetMyClasses()
        {
            var teacherIdStr = User.FindFirst("TeacherId")?.Value;
            if (string.IsNullOrEmpty(teacherIdStr))
                return Unauthorized("Teacher ID not found");

            var teacherId = int.Parse(teacherIdStr);
            var classes = _classService.GetClassesByTeacherId(teacherId);
            return Ok(_mapper.Map<List<ClassDto>>(classes));
        }

        // GET: api/teachers/me/classes/{classId}/students
        [HttpGet("me/classes/{classId}/students")]
        [Authorize(Roles = "Teacher")]
        public IActionResult GetStudentsByClass(int classId)
        {
            var teacherIdStr = User.FindFirst("TeacherId")?.Value;
            if (string.IsNullOrEmpty(teacherIdStr))
                return Unauthorized("Teacher ID not found");

            var teacherId = int.Parse(teacherIdStr);


            var classObj = _classService.GetById(classId);
            if (classObj == null)
                return NotFound("Class not found");

            if (classObj.ClassTeacherId != teacherId)
                return Forbid("You are not the teacher of this class");

            var students = _studentService.GetStudentsByClassId(classId);
            return Ok(_mapper.Map<List<StudentDto>>(students));
        }

        [HttpGet("count")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetCount()
        {
            return Ok(new { Count = _teacherService.GetAll().Count });
        }
    }
}