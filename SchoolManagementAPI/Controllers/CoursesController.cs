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
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IMapper _mapper;

        public CoursesController(ICourseService courseService, IMapper mapper)
        {
            _courseService = courseService;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            var courses = _courseService.GetAll();
            return Ok(_mapper.Map<List<CourseDto>>(courses));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var course = _courseService.GetById(id);
            if (course == null)
                return NotFound();
            return Ok(_mapper.Map<CourseDto>(course));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Add([FromBody] CreateCourseDto dto)
        {
            try
            {
                var course = _mapper.Map<Course>(dto);
                _courseService.Add(course);
                return Ok(_mapper.Map<CourseDto>(course));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UpdateCourseDto dto)
        {
            var existing = _courseService.GetById(id);
            if (existing == null)
                return NotFound("Course not found");

            _mapper.Map(dto, existing);

            var updated = _courseService.Update(existing);
            if (!updated)
                return NotFound("No changes were made");

            return Ok(_mapper.Map<CourseDto>(existing));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var deleted = _courseService.Delete(id);
            if (!deleted)
                return NotFound("Course not found");
            return Ok();
        }
        // GET: api/courses/paged?pageNumber=1&pageSize=10
        [HttpGet("paged")]
        [AllowAnonymous]
        public IActionResult GetPaged([FromQuery] PaginationParams paginationParams)
        {
            var pagedResult = _courseService.GetPaged(
                paginationParams.PageNumber,
                paginationParams.PageSize
            );

            var coursesDto = _mapper.Map<List<CourseDto>>(pagedResult.Data);

            var result = new PagedResult<CourseDto>
            {
                Data = coursesDto,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize,
                TotalCount = pagedResult.TotalCount,
                TotalPages = pagedResult.TotalPages
            };

            return Ok(result);
        }
        // GET: api/courses/search?keyword=math
        [HttpGet("search")]
        [AllowAnonymous]
        public IActionResult Search([FromQuery] string? keyword)
        {
            var courses = _courseService.Search(keyword);
            return Ok(_mapper.Map<List<CourseDto>>(courses));
        }

        // GET: api/courses/grade/1
        [HttpGet("grade/{gradeId}")]
        [AllowAnonymous]
        public IActionResult GetByGrade(int gradeId)
        {
            var courses = _courseService.GetCoursesByGradeId(gradeId);
            return Ok(_mapper.Map<List<CourseDto>>(courses));
        }

        // GET: api/courses/ordered-by-credits
        [HttpGet("ordered-by-credits")]
        [AllowAnonymous]
        public IActionResult GetOrderedByCredits()
        {
            var courses = _courseService.GetCoursesOrderedByCredits();
            return Ok(_mapper.Map<List<CourseDto>>(courses));
        }

        // GET: api/courses/credits-more-than/3
        [HttpGet("credits-more-than/{credits}")]
        [AllowAnonymous]
        public IActionResult GetWithCreditsMoreThan(int credits)
        {
            var courses = _courseService.GetCoursesWithCreditsMoreThan(credits);
            return Ok(_mapper.Map<List<CourseDto>>(courses));
        }

        // GET: api/courses/statistics/count-by-grade
        [HttpGet("statistics/count-by-grade")]
        [AllowAnonymous]
        public IActionResult GetCountByGrade()
        {
            return Ok(_courseService.GetCourseCountByGrade());
        }

        // GET: api/courses/statistics/average-credits
        [HttpGet("statistics/average-credits")]
        [AllowAnonymous]
        public IActionResult GetAverageCredits()
        {
            return Ok(new { AverageCredits = _courseService.GetAverageCredits() });
        }

        // GET: api/courses/statistics/max-credits
        [HttpGet("statistics/max-credits")]
        [AllowAnonymous]
        public IActionResult GetMaxCredits()
        {
            var course = _courseService.GetCourseWithMaxCredits();
            if (course == null) return NotFound();
            return Ok(_mapper.Map<CourseDto>(course));
        }

        // GET: api/courses/statistics/min-credits
        [HttpGet("statistics/min-credits")]
        [AllowAnonymous]
        public IActionResult GetMinCredits()
        {
            var course = _courseService.GetCourseWithMinCredits();
            if (course == null) return NotFound();
            return Ok(_mapper.Map<CourseDto>(course));
        }

        [HttpGet("count")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetCount()
        {
            return Ok(new { Count = _courseService.GetAll().Count });
        }
    }
}