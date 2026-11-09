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
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            var grades = _gradeService.GetAll();
            return Ok(_mapper.Map<List<GradeDto>>(grades));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetById(int id)
        {
            var grade = _gradeService.GetById(id);
            if (grade == null) return NotFound();
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
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UpdateGradeDto dto)
        {
            var existing = _gradeService.GetById(id);
            if (existing == null) return NotFound("Grade not found");
            _mapper.Map(dto, existing);
            _gradeService.Update(existing);
            return Ok(_mapper.Map<GradeDto>(existing));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var deleted = _gradeService.Delete(id);
            if (!deleted) return NotFound("Grade not found");
            return Ok();
        }

        [HttpGet("paged")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetPaged([FromQuery] PaginationParams paginationParams)
        {
            var pagedResult = _gradeService.GetPaged(paginationParams.PageNumber, paginationParams.PageSize);
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

        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public IActionResult Search([FromQuery] string? keyword)
        {
            var grades = _gradeService.Search(keyword);
            return Ok(_mapper.Map<List<GradeDto>>(grades));
        }

        [HttpGet("count")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetCount()
        {
            return Ok(new { Count = _gradeService.GetAll().Count });
        }
    }
}