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
    public class ClassesController : ControllerBase
    {
        private readonly IClassService _classService;
        private readonly IMapper _mapper;

        public ClassesController(IClassService classService, IMapper mapper)
        {
            _classService = classService;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            var classes = _classService.GetAll();
            return Ok(_mapper.Map<List<ClassDto>>(classes));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var classObj = _classService.GetById(id);
            if (classObj == null)
                return NotFound();
            return Ok(_mapper.Map<ClassDto>(classObj));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Add([FromBody] CreateClassDto dto)
        {
            try
            {
                var classObj = _mapper.Map<Class>(dto);
                _classService.Add(classObj);
                return Ok(_mapper.Map<ClassDto>(classObj));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UpdateClassDto dto)
        {
            var existing = _classService.GetById(id);
            if (existing == null)
                return NotFound("Class not found");

            _mapper.Map(dto, existing);

            var updated = _classService.Update(existing);
            if (!updated)
                return NotFound("No changes were made");

            return Ok(_mapper.Map<ClassDto>(existing));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var deleted = _classService.Delete(id);
            if (!deleted)
                return NotFound("Class not found");
            return Ok();
        }


        // GET: api/classes/paged?pageNumber=1&pageSize=10
        [HttpGet("paged")]
        [AllowAnonymous]
        public IActionResult GetPaged([FromQuery] PaginationParams paginationParams)
        {
            var pagedResult = _classService.GetPaged(
                paginationParams.PageNumber,
                paginationParams.PageSize
            );

            var classesDto = _mapper.Map<List<ClassDto>>(pagedResult.Data);

            var result = new PagedResult<ClassDto>
            {
                Data = classesDto,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize,
                TotalCount = pagedResult.TotalCount,
                TotalPages = pagedResult.TotalPages
            };

            return Ok(result);
        }
        // GET: api/classes/search?keyword=3A
        [HttpGet("search")]
        [AllowAnonymous]
        public IActionResult Search([FromQuery] string? keyword)
        {
            var classes = _classService.Search(keyword);
            return Ok(_mapper.Map<List<ClassDto>>(classes));
        }

        // GET: api/classes/ordered
        [HttpGet("ordered")]
        [AllowAnonymous]
        public IActionResult GetOrdered()
        {
            var classes = _classService.GetClassesOrderedByName();
            return Ok(_mapper.Map<List<ClassDto>>(classes));
        }

        // GET: api/classes/teacher/1
        [HttpGet("teacher/{teacherId}")]
        [AllowAnonymous]
        public IActionResult GetByTeacher(int teacherId)
        {
            var classes = _classService.GetClassesByTeacherId(teacherId);
            return Ok(_mapper.Map<List<ClassDto>>(classes));
        }

        // GET: api/classes/statistics/count-by-grade
        [HttpGet("statistics/count-by-grade")]
        [AllowAnonymous]
        public IActionResult GetCountByGrade()
        {
            return Ok(_classService.GetClassCountByGrade());
        }

        // GET: api/classes/statistics/student-count
        [HttpGet("statistics/student-count")]
        [AllowAnonymous]
        public IActionResult GetStudentCount()
        {
            return Ok(_classService.GetStudentCountByClass());
        }

        // GET: api/classes/statistics/most-students
        [HttpGet("statistics/most-students")]
        [AllowAnonymous]
        public IActionResult GetMostStudents()
        {
            var classObj = _classService.GetClassWithMostStudents();
            if (classObj == null) return NotFound();
            return Ok(_mapper.Map<ClassDto>(classObj));
        }

        // GET: api/classes/statistics/least-students
        [HttpGet("statistics/least-students")]
        [AllowAnonymous]
        public IActionResult GetLeastStudents()
        {
            var classObj = _classService.GetClassWithLeastStudents();
            if (classObj == null) return NotFound();
            return Ok(_mapper.Map<ClassDto>(classObj));
        }
    }
}