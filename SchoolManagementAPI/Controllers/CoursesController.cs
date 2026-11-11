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
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IClassService _classService;
        private readonly IMapper _mapper;

        public CoursesController(ICourseService courseService, IClassService classService, IMapper mapper)
        {
            _courseService = courseService;
            _classService = classService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            var courses = _courseService.GetAll();
            return Ok(_mapper.Map<List<CourseDto>>(courses));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetById(int id)
        {
            var course = _courseService.GetById(id);
            if (course == null) return NotFound();
            return Ok(_mapper.Map<CourseDto>(course));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Add([FromBody] CreateCourseDto dto)
        {
            try
            {
                if (dto.ClassId <= 0) return BadRequest("ClassId is required");
                var assignedClass = _classService.GetById(dto.ClassId);
                if (assignedClass == null) return BadRequest("Class not found");
                if (assignedClass.GradeId != dto.GradeId) return BadRequest("Course grade must match the assigned class grade");
                var course = _mapper.Map<Course>(dto);
                _courseService.Add(course);
                return Ok(_mapper.Map<CourseDto>(course));
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UpdateCourseDto dto)
        {
            var existing = _courseService.GetById(id);
            if (existing == null) return NotFound("Course not found");
            if (dto.ClassId <= 0) return BadRequest("ClassId is required");
            var assignedClass = _classService.GetById(dto.ClassId);
            if (assignedClass == null) return BadRequest("Class not found");
            if (assignedClass.GradeId != dto.GradeId) return BadRequest("Course grade must match the assigned class grade");
            _mapper.Map(dto, existing);
            _courseService.Update(existing);
            return Ok(_mapper.Map<CourseDto>(existing));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var deleted = _courseService.Delete(id);
            if (!deleted) return NotFound("Course not found");
            return Ok();
        }

        [HttpGet("paged")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetPaged([FromQuery] PaginationParams paginationParams)
        {
            var pagedResult = _courseService.GetPaged(paginationParams.PageNumber, paginationParams.PageSize);
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

        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public IActionResult Search([FromQuery] string? keyword)
        {
            var courses = _courseService.Search(keyword);
            return Ok(_mapper.Map<List<CourseDto>>(courses));
        }

        [HttpGet("count")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetCount()
        {
            return Ok(new { Count = _courseService.GetAll().Count });
        }
    }
}
