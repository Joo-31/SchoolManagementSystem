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
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            var classes = _classService.GetAll();
            return Ok(_mapper.Map<List<ClassDto>>(classes));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult GetById(int id)
        {
            var classObj = _classService.GetById(id);
            if (classObj == null) return NotFound();
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
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UpdateClassDto dto)
        {
            var existing = _classService.GetById(id);
            if (existing == null) return NotFound("Class not found");
            _mapper.Map(dto, existing);
            _classService.Update(existing);
            return Ok(_mapper.Map<ClassDto>(existing));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var deleted = _classService.Delete(id);
            if (!deleted) return NotFound("Class not found");
            return Ok();
        }

        [HttpGet("paged")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetPaged([FromQuery] PaginationParams paginationParams)
        {
            var pagedResult = _classService.GetPaged(paginationParams.PageNumber, paginationParams.PageSize);
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

        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public IActionResult Search([FromQuery] string? keyword)
        {
            var classes = _classService.Search(keyword);
            return Ok(_mapper.Map<List<ClassDto>>(classes));
        }

        [HttpGet("count")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetCount()
        {
            return Ok(new { Count = _classService.GetAll().Count });
        }
    }
}