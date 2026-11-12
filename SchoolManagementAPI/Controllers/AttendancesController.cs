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
    public class AttendancesController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IMapper _mapper;
        private readonly IClassService _classService;
        private readonly ICourseService _courseService;
        private readonly IStudentService _studentService;

        public AttendancesController(IAttendanceService attendanceService, IMapper mapper, IClassService classService, ICourseService courseService, IStudentService studentService)
        {
            _attendanceService = attendanceService;
            _mapper = mapper;
            _classService = classService;
            _courseService = courseService;
            _studentService = studentService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetAll()
        {
            var attendances = _attendanceService.GetAll();
            if (User.IsInRole("Teacher"))
                attendances = attendances.Where(a => CanAccessAttendance(a, GetTeacherId())).ToList();
            return Ok(_mapper.Map<List<AttendanceDto>>(attendances));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetById(int id)
        {
            var attendance = _attendanceService.GetById(id);
            if (attendance == null) return NotFound();
            if (User.IsInRole("Teacher") && !CanAccessAttendance(attendance, GetTeacherId())) return Forbid();
            return Ok(_mapper.Map<AttendanceDto>(attendance));
        }

        [HttpGet("class/{classId}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetByClassId(int classId)
        {
            var classObj = _classService.GetById(classId);
            if (classObj == null) return NotFound("Class not found");

            if (User.IsInRole("Teacher") && !IsTeacherAssignedToClass(classObj, GetTeacherId()))
                return Forbid();

            var attendances = _attendanceService.SearchByClassId(classId);
            return Ok(_mapper.Map<List<AttendanceDto>>(attendances));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Add([FromBody] CreateAttendanceDto dto)
        {
            if (User.IsInRole("Teacher"))
            {
                var classObj = _classService.GetById(dto.ClassId);
                if (classObj == null) return NotFound("Class not found");
                if (!IsTeacherAssignedToClass(classObj, GetTeacherId())) return Forbid();
                if (!_studentService.GetStudentsByClassId(dto.ClassId).Any(s => s.Id == dto.StudentId))
                    return BadRequest("Student does not belong to this class");
            }
            try
            {
                var attendance = _mapper.Map<Attendance>(dto);
                _attendanceService.Add(attendance);
                return Ok(_mapper.Map<AttendanceDto>(attendance));
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Update(int id, [FromBody] UpdateAttendanceDto dto)
        {
            var existing = _attendanceService.GetById(id);
            if (existing == null) return NotFound("Attendance not found");
            if (User.IsInRole("Teacher"))
            {
                if (!CanAccessAttendance(existing, GetTeacherId())) return Forbid();
                var targetClass = _classService.GetById(dto.ClassId);
                if (targetClass == null) return NotFound("Class not found");
                if (!IsTeacherAssignedToClass(targetClass, GetTeacherId()) ||
                    !_studentService.GetStudentsByClassId(dto.ClassId).Any(s => s.Id == dto.StudentId)) return Forbid();
            }
            _mapper.Map(dto, existing);
            _attendanceService.Update(existing);
            return Ok(_mapper.Map<AttendanceDto>(existing));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var deleted = _attendanceService.Delete(id);
            if (!deleted) return NotFound("Attendance not found");
            return Ok();
        }

        [HttpGet("paged")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetPaged([FromQuery] PaginationParams paginationParams)
        {
            var pagedResult = _attendanceService.GetPaged(paginationParams.PageNumber, paginationParams.PageSize);
            var data = User.IsInRole("Teacher") ? pagedResult.Data.Where(a => CanAccessAttendance(a, GetTeacherId())).ToList() : pagedResult.Data;
            var attendancesDto = _mapper.Map<List<AttendanceDto>>(data);
            var result = new PagedResult<AttendanceDto>
            {
                Data = attendancesDto,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize,
                TotalCount = data.Count,
                TotalPages = (int)Math.Ceiling(data.Count / (double)pagedResult.PageSize)
            };
            return Ok(result);
        }

        [HttpGet("filter")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetPagedWithFilters(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? date = null,
            [FromQuery] int? classId = null,
            [FromQuery] string? status = null)
        {
            bool? isPresent = null;
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status.Equals("present", StringComparison.OrdinalIgnoreCase)) isPresent = true;
                else if (status.Equals("absent", StringComparison.OrdinalIgnoreCase)) isPresent = false;
            }

            var pagedResult = _attendanceService.GetPagedWithFilters(pageNumber, pageSize, date, classId, isPresent);
            if (User.IsInRole("Teacher") && classId.HasValue)
            {
                var classObj = _classService.GetById(classId.Value);
                if (classObj == null) return NotFound("Class not found");
                if (!IsTeacherAssignedToClass(classObj, GetTeacherId())) return Forbid();
            }
            var data = User.IsInRole("Teacher") ? pagedResult.Data.Where(a => CanAccessAttendance(a, GetTeacherId())).ToList() : pagedResult.Data;
            var attendancesDto = _mapper.Map<List<AttendanceDto>>(data);
            var result = new PagedResult<AttendanceDto>
            {
                Data = attendancesDto,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize,
                TotalCount = data.Count,
                TotalPages = (int)Math.Ceiling(data.Count / (double)pageSize)
            };
            return Ok(result);
        }

        [HttpPost("take")]
        [Authorize(Roles = "Teacher")]
        public IActionResult TakeAttendance([FromBody] TakeAttendanceDto dto)
        {
            var teacherIdStr = User.FindFirst("TeacherId")?.Value;
            if (string.IsNullOrEmpty(teacherIdStr)) return Unauthorized("Teacher ID not found");
            if (!int.TryParse(teacherIdStr, out var teacherId)) return Unauthorized("Invalid Teacher ID");

            var classObj = _classService.GetById(dto.ClassId);
            if (classObj == null) return NotFound("Class not found");
            if (!IsTeacherAssignedToClass(classObj, teacherId)) return Forbid();

            foreach (var record in dto.Records)
            {
                if (!_studentService.GetStudentsByClassId(dto.ClassId).Any(s => s.Id == record.StudentId))
                    return BadRequest("Student does not belong to this class");
                var existing = _attendanceService.GetAttendanceByStudentIdAndDate(record.StudentId, dto.Date.Date)
                    .FirstOrDefault(a => a.ClassId == dto.ClassId);
                if (existing != null)
                {
                    existing.IsPresent = record.IsPresent;
                    _attendanceService.Update(existing);
                }
                else
                {
                    _attendanceService.Add(new Attendance
                    {
                        StudentId = record.StudentId,
                        ClassId = dto.ClassId,
                        Date = dto.Date.Date,
                        IsPresent = record.IsPresent
                    });
                }
            }
            return Ok(new { message = "Attendance recorded successfully" });
        }

        private int GetTeacherId() => int.TryParse(User.FindFirst("TeacherId")?.Value, out var teacherId) ? teacherId : 0;

        private bool CanAccessAttendance(Attendance attendance, int teacherId)
        {
            var classObj = _classService.GetById(attendance.ClassId);
            return classObj != null && IsTeacherAssignedToClass(classObj, teacherId);
        }

        private bool IsTeacherAssignedToClass(Class classObj, int teacherId) =>
            teacherId > 0 && (classObj.ClassTeacherId == teacherId ||
                _courseService.GetAll().Any(c => c.TeacherId == teacherId && c.ClassId == classObj.Id));

        public class TakeAttendanceDto
        {
            public int ClassId { get; set; }
            public DateTime Date { get; set; }
            public List<AttendanceRecordDto> Records { get; set; } = new();
        }

        public class AttendanceRecordDto
        {
            public int StudentId { get; set; }
            public bool IsPresent { get; set; }
        }

        [HttpGet("statistics/percentage/{date}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetPercentage(DateTime date)
        {
            var percentage = _attendanceService.GetAttendancePercentageByDate(date);

            return Ok(new { percentage = Math.Round(percentage, 1) });
        }
    }
}
