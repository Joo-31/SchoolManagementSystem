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
            if (attendance == null) return NotFound();
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
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult Update(int id, [FromBody] UpdateAttendanceDto dto)
        {
            var existing = _attendanceService.GetById(id);
            if (existing == null) return NotFound("Attendance not found");
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

        [HttpPost("take")]
        [Authorize(Roles = "Teacher")]
        public IActionResult TakeAttendance([FromBody] TakeAttendanceDto dto)
        {
            var teacherIdStr = User.FindFirst("TeacherId")?.Value;
            if (string.IsNullOrEmpty(teacherIdStr)) return Unauthorized("Teacher ID not found");
            var teacherId = int.Parse(teacherIdStr);

            var classObj = _classService.GetById(dto.ClassId);
            if (classObj == null) return NotFound("Class not found");
            if (classObj.ClassTeacherId != teacherId) return StatusCode(403, new { message = "You are not the teacher of this class" });

            foreach (var record in dto.Records)
            {
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
