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
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;
        private readonly IAttendanceService _attendanceService;

        public StudentsController(IStudentService studentService, IAttendanceService attendanceService, IMapper mapper)
        {
            _studentService = studentService;
            _mapper = mapper;
            _attendanceService = attendanceService;
        }

        // GET: api/students
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            var students = _studentService.GetAll();
            var studentsDto = _mapper.Map<List<StudentDto>>(students);
            return Ok(studentsDto);
        }

        // GET: api/students/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var student = _studentService.GetById(id);
            if (student == null)
                return NotFound();

            var studentDto = _mapper.Map<StudentDto>(student);
            return Ok(studentDto);
        }

        // POST: api/students
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Add([FromBody] CreateStudentDto dto)
        {
            try
            {
                var student = _mapper.Map<Student>(dto);
                _studentService.Add(student);

                var studentDto = _mapper.Map<StudentDto>(student);
                return Ok(studentDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/students/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateStudentDto dto)
        {
            var existing = _studentService.GetById(id);
            if (existing == null)
                return NotFound("Student not found");

            _mapper.Map(dto, existing);

            var updated = _studentService.Update(existing);
            if (!updated)
                return Ok(existing);  // ← بدل NotFound

            return Ok(_mapper.Map<StudentDto>(existing));
        }

        // DELETE: api/students/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var deleted = _studentService.Delete(id);
            if (!deleted)
                return NotFound("Student not found");

            return Ok();
        }

        // GET: api/students/paged?pageNumber=1&pageSize=10
        [HttpGet("paged")]
        [AllowAnonymous]
        public IActionResult GetPaged([FromQuery] PaginationParams paginationParams)
        {
            var pagedResult = _studentService.GetPaged(
                paginationParams.PageNumber,
                paginationParams.PageSize
            );

            // ✅ حول الـ Students لـ StudentDto
            var studentsDto = _mapper.Map<List<StudentDto>>(pagedResult.Data);

            var result = new PagedResult<StudentDto>
            {
                Data = studentsDto,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize,
                TotalCount = pagedResult.TotalCount,
                TotalPages = pagedResult.TotalPages
            };

            return Ok(result);
        }

        // GET: api/students/search?keyword=ahmed
        [HttpGet("search")]
        [AllowAnonymous]
        public IActionResult Search([FromQuery] string? keyword)
        {
            var students = _studentService.Search(keyword);
            return Ok(_mapper.Map<List<StudentDto>>(students));
        }

        // GET: api/students/class/5
        [HttpGet("class/{classId}")]
        [AllowAnonymous]
        public IActionResult GetByClass(int classId)
        {
            var students = _studentService.GetStudentsByClassId(classId);
            return Ok(_mapper.Map<List<StudentDto>>(students));
        }

        // GET: api/students/ordered
        [HttpGet("ordered")]
        [AllowAnonymous]
        public IActionResult GetOrdered()
        {
            var students = _studentService.GetStudentsOrderedByName();
            return Ok(_mapper.Map<List<StudentDto>>(students));
        }

        // GET: api/students/older-than/15
        [HttpGet("older-than/{age}")]
        [AllowAnonymous]
        public IActionResult GetOlderThan(int age)
        {
            var students = _studentService.GetStudentsOlderThan(age);
            return Ok(_mapper.Map<List<StudentDto>>(students));
        }

        // GET: api/students/statistics/average-age
        [HttpGet("statistics/average-age")]
        [AllowAnonymous]
        public IActionResult GetAverageAge()
        {
            return Ok(new { AverageAge = _studentService.GetAverageAge() });
        }

        // GET: api/students/statistics/oldest
        [HttpGet("statistics/oldest")]
        [AllowAnonymous]
        public IActionResult GetOldest()
        {
            var student = _studentService.GetOldestStudent();
            if (student == null) return NotFound();
            return Ok(_mapper.Map<StudentDto>(student));
        }

        // GET: api/students/statistics/youngest
        [HttpGet("statistics/youngest")]
        [AllowAnonymous]
        public IActionResult GetYoungest()
        {
            var student = _studentService.GetYoungestStudent();
            if (student == null) return NotFound();
            return Ok(_mapper.Map<StudentDto>(student));
        }

        // GET: api/students/statistics/count-by-class
        [HttpGet("statistics/count-by-class")]
        [AllowAnonymous]
        public IActionResult GetCountByClass()
        {
            return Ok(_studentService.GetStudentCountByClass());
        }

        // GET: api/students/class/1/oldest
        [HttpGet("class/{classId}/oldest")]
        [AllowAnonymous]
        public IActionResult GetOldestByClass(int classId)
        {
            var student = _studentService.GetOldestStudentByClass(classId);
            if (student == null) return NotFound("No students in this class");
            return Ok(_mapper.Map<StudentDto>(student));
        }

        // GET: api/students/class/1/youngest
        [HttpGet("class/{classId}/youngest")]
        [AllowAnonymous]
        public IActionResult GetYoungestByClass(int classId)
        {
            var student = _studentService.GetYoungestStudentByClass(classId);
            if (student == null) return NotFound("No students in this class");
            return Ok(_mapper.Map<StudentDto>(student));
        }

        // GET: api/students/me
        [HttpGet("me")]
        [Authorize(Roles = "Student")]
        public IActionResult GetMe()
        {
            var studentIdStr = User.FindFirst("StudentId")?.Value;
            if (string.IsNullOrEmpty(studentIdStr))
                return Unauthorized("Student ID not found");

            var studentId = int.Parse(studentIdStr);
            var student = _studentService.GetById(studentId);

            if (student == null)
                return NotFound();

            return Ok(_mapper.Map<StudentDto>(student));
        }

        // GET: api/students/me/attendances
        [HttpGet("me/attendances")]
        [Authorize(Roles = "Student")]
        public IActionResult GetMyAttendances()
        {
            var studentIdStr = User.FindFirst("StudentId")?.Value;
            if (string.IsNullOrEmpty(studentIdStr))
                return Unauthorized("Student ID not found");

            var studentId = int.Parse(studentIdStr);
            var attendances = _attendanceService.SearchByStudentId(studentId);

            return Ok(_mapper.Map<List<AttendanceDto>>(attendances));
        }

    }
}