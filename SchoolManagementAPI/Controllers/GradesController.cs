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
    public class GradesController : ControllerBase
    {
        private readonly IGradeService _gradeService;
        private readonly IMapper _mapper;

        public GradesController(IGradeService gradeService, IMapper mapper)
        {
            _gradeService = gradeService;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            var grades = _gradeService.GetAll();
            return Ok(_mapper.Map<List<GradeDto>>(grades));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var grade = _gradeService.GetById(id);
            if (grade == null)
                return NotFound();
            return Ok(_mapper.Map<GradeDto>(grade));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Add([FromBody] CreateGradeDto dto)
        {
            try
            {
                var grade = _mapper.Map<Grade>(dto);
                _gradeService.Add(grade);
                return Ok(_mapper.Map<GradeDto>(grade));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UpdateGradeDto dto)
        {
            var existing = _gradeService.GetById(id);
            if (existing == null)
                return NotFound("Grade not found");

            _mapper.Map(dto, existing);

            var updated = _gradeService.Update(existing);
            if (!updated)
                return NotFound("No changes were made");

            return Ok(_mapper.Map<GradeDto>(existing));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var deleted = _gradeService.Delete(id);
            if (!deleted)
                return NotFound("Grade not found");
            return Ok();
        }
        // GET: api/grades/paged?pageNumber=1&pageSize=10
        [HttpGet("paged")]
        [AllowAnonymous]
        public IActionResult GetPaged([FromQuery] PaginationParams paginationParams)
        {
            var pagedResult = _gradeService.GetPaged(
                paginationParams.PageNumber,
                paginationParams.PageSize
            );

            var gradesDto = _mapper.Map<List<GradeDto>>(pagedResult.Data);

            var result = new PagedResult<GradeDto>
            {
                Data = gradesDto,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize,
                TotalCount = pagedResult.TotalCount,
                TotalPages = pagedResult.TotalPages
            };

            return Ok(result);
        }

        // GET: api/grades/search?keyword=primary
        [HttpGet("search")]
        [AllowAnonymous]
        public IActionResult Search([FromQuery] string? keyword)
        {
            var grades = _gradeService.Search(keyword);
            return Ok(_mapper.Map<List<GradeDto>>(grades));
        }

        // GET: api/grades/ordered
        [HttpGet("ordered")]
        [AllowAnonymous]
        public IActionResult GetOrdered()
        {
            var grades = _gradeService.GetGradesOrderedByLevel();
            return Ok(_mapper.Map<List<GradeDto>>(grades));
        }

        // GET: api/grades/statistics/highest-level
        [HttpGet("statistics/highest-level")]
        [AllowAnonymous]
        public IActionResult GetHighestLevel()
        {
            var grade = _gradeService.GetHighestLevelGrade();
            if (grade == null) return NotFound();
            return Ok(_mapper.Map<GradeDto>(grade));
        }

        // GET: api/grades/statistics/lowest-level
        [HttpGet("statistics/lowest-level")]
        [AllowAnonymous]
        public IActionResult GetLowestLevel()
        {
            var grade = _gradeService.GetLowestLevelGrade();
            if (grade == null) return NotFound();
            return Ok(_mapper.Map<GradeDto>(grade));
        }

        // GET: api/grades/statistics/class-count
        [HttpGet("statistics/class-count")]
        [AllowAnonymous]
        public IActionResult GetClassCount()
        {
            return Ok(_gradeService.GetClassCountByGradeName());
        }

        // GET: api/grades/statistics/course-count
        [HttpGet("statistics/course-count")]
        [AllowAnonymous]
        public IActionResult GetCourseCount()
        {
            return Ok(_gradeService.GetCourseCountByGradeName());
        }

        // GET: api/grades/statistics/student-count
        [HttpGet("statistics/student-count")]
        [AllowAnonymous]
        public IActionResult GetStudentCount()
        {
            return Ok(_gradeService.GetStudentCountByGradeName());
        }
    }
}