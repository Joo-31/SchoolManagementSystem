using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementAPI.DTOs.Requests;
using SchoolManagementAPI.DTOs.Responses;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Services.Interfaces;

namespace SchoolManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttendancesController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IMapper _mapper;
        private readonly IClassService _classService;

        public AttendancesController(IAttendanceService attendanceService, IMapper mapper, IClassService classService)
        {
            _attendanceService = attendanceService;
            _mapper = mapper;
            _classService = classService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetAll()
        {
            var attendances = _attendanceService.GetAll();
            return Ok(_mapper.Map<List<AttendanceDto>>(attendances));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetById(int id)
        {
            var attendance = _attendanceService.GetById(id);
            if (attendance == null)
                return NotFound();
            return Ok(_mapper.Map<AttendanceDto>(attendance));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Add([FromBody] CreateAttendanceDto dto)
        {
            try
            {
                var attendance = _mapper.Map<Attendance>(dto);
                _attendanceService.Add(attendance);
                return Ok(_mapper.Map<AttendanceDto>(attendance));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Update(int id, [FromBody] UpdateAttendanceDto dto)
        {
            var existing = _attendanceService.GetById(id);
            if (existing == null)
                return NotFound("Attendance not found");

            _mapper.Map(dto, existing);

            var updated = _attendanceService.Update(existing);
            if (!updated)
                return NotFound("No changes were made");

            return Ok(_mapper.Map<AttendanceDto>(existing));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var deleted = _attendanceService.Delete(id);
            if (!deleted)
                return NotFound("Attendance not found");
            return Ok();
        }

        [HttpGet("student/{studentId}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetByStudent(int studentId)
        {
            var attendances = _attendanceService.SearchByStudentId(studentId);
            return Ok(_mapper.Map<List<AttendanceDto>>(attendances));
        }

        [HttpGet("class/{classId}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetByClass(int classId)
        {
            var attendances = _attendanceService.SearchByClassId(classId);
            return Ok(_mapper.Map<List<AttendanceDto>>(attendances));
        }


        // GET: api/attendances/paged?pageNumber=1&pageSize=10
        [HttpGet("paged")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetPaged([FromQuery] PaginationParams paginationParams)
        {
            var pagedResult = _attendanceService.GetPaged(
                paginationParams.PageNumber,
                paginationParams.PageSize
            );

            var attendancesDto = _mapper.Map<List<AttendanceDto>>(pagedResult.Data);

            var result = new PagedResult<AttendanceDto>
            {
                Data = attendancesDto,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize,
                TotalCount = pagedResult.TotalCount,
                TotalPages = pagedResult.TotalPages
            };

            return Ok(result);
        }
        // GET: api/attendances/date/2026-09-19
        [HttpGet("date/{date}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetByDate(DateTime date)
        {
            var attendances = _attendanceService.SearchByDate(date);
            return Ok(_mapper.Map<List<AttendanceDto>>(attendances));
        }

        // GET: api/attendances/student/5/date/2026-09-19
        [HttpGet("student/{studentId}/date/{date}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetByStudentAndDate(int studentId, DateTime date)
        {
            var attendances = _attendanceService.GetAttendanceByStudentIdAndDate(studentId, date);
            return Ok(_mapper.Map<List<AttendanceDto>>(attendances));
        }

        // GET: api/attendances/statistics/present/2026-09-19
        [HttpGet("statistics/present/{date}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetPresentCount(DateTime date)
        {
            return Ok(new { PresentCount = _attendanceService.GetPresentCountByDate(date) });
        }

        // GET: api/attendances/statistics/absent/2026-09-19
        [HttpGet("statistics/absent/{date}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetAbsentCount(DateTime date)
        {
            return Ok(new { AbsentCount = _attendanceService.GetAbsentCountByDate(date) });
        }

        // GET: api/attendances/statistics/percentage/2026-09-19
        [HttpGet("statistics/percentage/{date}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetPercentage(DateTime date)
        {
            return Ok(new { Percentage = _attendanceService.GetAttendancePercentageByDate(date) });
        }

        // GET: api/attendances/statistics/absent-count/5
        [HttpGet("statistics/absent-count/{studentId}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetAbsentCountByStudent(int studentId)
        {
            return Ok(new { AbsentCount = _attendanceService.GetAbsentCountByStudent(studentId) });
        }

        // GET: api/attendances/statistics/most-absent
        [HttpGet("statistics/most-absent")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetMostAbsent()
        {
            var studentId = _attendanceService.GetStudentWithMostAbsences();
            if (studentId == null) return NotFound();
            return Ok(new { StudentId = studentId });
        }

        // GET: api/attendances/statistics/class-count
        [HttpGet("statistics/class-count")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetCountByClass()
        {
            return Ok(_attendanceService.GetAttendanceCountByClass());
        }

        // POST: api/attendances/take
        [HttpPost("take")]
        [Authorize(Roles = "Teacher")]
        public IActionResult TakeAttendance([FromBody] TakeAttendanceDto dto)
        {
            var teacherIdStr = User.FindFirst("TeacherId")?.Value;
            if (string.IsNullOrEmpty(teacherIdStr))
                return Unauthorized("Teacher ID not found");

            var teacherId = int.Parse(teacherIdStr);

            var classObj = _classService.GetById(dto.ClassId);
            if (classObj == null)
                return NotFound("Class not found");

            if (classObj.ClassTeacherId != teacherId)
                return Forbid("You are not the teacher of this class");

            // ✅ سجل حضور كل طالب
            foreach (var record in dto.Records)
            {
                // شوف لو الطالب عنده Attendance في اليوم ده
                var existing = _attendanceService
                    .GetAttendanceByStudentIdAndDate(record.StudentId, dto.Date.Date)
                    .FirstOrDefault(attendance => attendance.ClassId == dto.ClassId);

                if (existing != null)
                {
                    // ✅ Update
                    existing.IsPresent = record.IsPresent;
                    _attendanceService.Update(existing);
                    Console.WriteLine($"✅ Updated: Student={record.StudentId}, Present={record.IsPresent}");
                }
                else
                {
                    // ✅ Add
                    var attendance = new Attendance
                    {
                        StudentId = record.StudentId,
                        ClassId = dto.ClassId,
                        Date = dto.Date.Date,
                        IsPresent = record.IsPresent
                    };
                    _attendanceService.Add(attendance);
                    Console.WriteLine($"✅ Added: Student={record.StudentId}, Present={record.IsPresent}");
                }
            }

            return Ok(new { message = "Attendance recorded successfully" });
        }

        // DTO
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
    }
}