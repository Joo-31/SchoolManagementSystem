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

        public MarksController(
            IMarkService markService,
            ICourseService courseService,
            IClassService classService,
            IStudentService studentService,
            IMapper mapper)
        {
            _markService = markService;
            _courseService = courseService;
            _classService = classService;
            _studentService = studentService;
            _mapper = mapper;
        }

        // Admin + Teacher
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

        // Teacher + Admin
        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Add([FromBody] CreateMarkDto dto)
        {
            try
            {
                // ✅ لو Teacher — نستخدم TeacherId من التوكن
                int teacherId;
                if (User.IsInRole("Teacher"))
                {
                    var teacherIdStr = User.FindFirst("TeacherId")?.Value;
                    if (string.IsNullOrEmpty(teacherIdStr))
                        return Unauthorized("Teacher ID not found");

                    teacherId = int.Parse(teacherIdStr);
                }
                else
                {
                    // Admin — لازم يبعت TeacherId
                    if (dto.TeacherId <= 0)
                        return BadRequest("TeacherId is required");
                    teacherId = dto.TeacherId;
                }

                // ✅ تأكد إن المدرس ده بيدرس المادة دي
                var course = _courseService.GetById(dto.CourseId);
                if (course == null)
                    return NotFound("Course not found");

                if (course.TeacherId != teacherId)
                    return StatusCode(403, new { message = "You are not the teacher of this course" });
                // ✅ تأكد إن الطالب معندوش Mark في نفس المادة
                var existingMarks = _markService.GetByStudentId(dto.StudentId);
                var duplicate = existingMarks.FirstOrDefault(m => m.CourseId == dto.CourseId);

                if (duplicate != null)
                    return BadRequest("Student already has a mark for this course. Please update it instead.");

                // ✅ اعمل Mark
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Update(int id, [FromBody] UpdateMarkDto dto)
        {
            var existing = _markService.GetById(id);
            if (existing == null) return NotFound();

            _mapper.Map(dto, existing);
            var updated = _markService.Update(existing);
            if (!updated) return NotFound("No changes made");

            // ✅ هات الـ Mark تاني بعد الحفظ
            var savedMark = _markService.GetById(id);
            return Ok(_mapper.Map<MarkDto>(savedMark));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Delete(int id)
        {
            var mark = _markService.GetById(id);
            if (mark == null) return NotFound();

            // ✅ لو Teacher — لازم يكون هو اللي حط الـ Mark
            if (User.IsInRole("Teacher"))
            {
                var teacherIdStr = User.FindFirst("TeacherId")?.Value;
                if (string.IsNullOrEmpty(teacherIdStr))
                    return Unauthorized();

                var teacherId = int.Parse(teacherIdStr);
                if (mark.TeacherId != teacherId)
                    return Forbid("You can only delete your own marks");
            }

            var deleted = _markService.Delete(id);
            if (!deleted) return NotFound();

            return Ok(new { message = "Mark deleted successfully" });
        }

        // Student — يشوف درجاته
        [HttpGet("me")]
        [Authorize(Roles = "Student")]
        public IActionResult GetMyMarks()
        {
            var studentIdStr = User.FindFirst("StudentId")?.Value;
            if (string.IsNullOrEmpty(studentIdStr))
                return Unauthorized("Student ID not found");

            var studentId = int.Parse(studentIdStr);
            var marks = _markService.GetByStudentId(studentId);
            return Ok(_mapper.Map<List<MarkDto>>(marks));
        }

        // Teacher — يشوف درجات اللي هو حطها
        [HttpGet("my-marks")]
        [Authorize(Roles = "Teacher")]
        public IActionResult GetMarksByTeacher()
        {
            var teacherIdStr = User.FindFirst("TeacherId")?.Value;
            if (string.IsNullOrEmpty(teacherIdStr))
                return Unauthorized("Teacher ID not found");

            var teacherId = int.Parse(teacherIdStr);
            var marks = _markService.GetByTeacherId(teacherId);
            return Ok(_mapper.Map<List<MarkDto>>(marks));
        }

        // Course marks
        [HttpGet("course/{courseId}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetByCourse(int courseId)
        {
            var marks = _markService.GetByCourseId(courseId);
            return Ok(_mapper.Map<List<MarkDto>>(marks));
        }

        // Student marks by id (Admin/Teacher)
        [HttpGet("student/{studentId}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetByStudent(int studentId)
        {
            var marks = _markService.GetByStudentId(studentId);
            return Ok(_mapper.Map<List<MarkDto>>(marks));
        }

        // GET: api/marks/my-courses
        [HttpGet("my-courses")]
        [Authorize(Roles = "Teacher")]
        public IActionResult GetMyCourses()
        {
            var teacherIdStr = User.FindFirst("TeacherId")?.Value;
            if (string.IsNullOrEmpty(teacherIdStr))
                return Unauthorized();

            var teacherId = int.Parse(teacherIdStr);
            var courses = _courseService.GetAll().Where(c => c.TeacherId == teacherId).ToList();
            return Ok(_mapper.Map<List<CourseDto>>(courses));
        }

        // GET: api/marks/my-students
        [HttpGet("my-students")]
        [Authorize(Roles = "Teacher")]
        public IActionResult GetMyStudents()
        {
            var teacherIdStr = User.FindFirst("TeacherId")?.Value;
            if (string.IsNullOrEmpty(teacherIdStr))
                return Unauthorized();

            var teacherId = int.Parse(teacherIdStr);

            // جيب فصول المدرس
            var classService = HttpContext.RequestServices.GetService<IClassService>();
            var studentService = HttpContext.RequestServices.GetService<IStudentService>();

            var classes = classService!.GetClassesByTeacherId(teacherId);

            var students = new List<Student>();
            foreach (var c in classes)
            {
                students.AddRange(studentService!.GetStudentsByClassId(c.Id));
            }

            return Ok(_mapper.Map<List<StudentDto>>(students));
        }
    }
}