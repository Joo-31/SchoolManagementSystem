using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementAPI.DTOs.Requests;
using SchoolManagementAPI.DTOs.Responses;
using SchoolManagementAPI.Repositories.Interfaces;

namespace SchoolManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // 🔒 الأدمن بس هو اللي يقدر يدخل هنا
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UsersController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/users
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _unitOfWork.Users.GetAll();
            // ✅ بنرجع UserDto عشان الباسورد ميبانش
            return Ok(_mapper.Map<List<UserDto>>(users));
        }

        // PUT: api/users/5/reset-password
        [HttpPut("{id}/reset-password")]
        public IActionResult ResetPassword(int id, [FromBody] ResetPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 6)
                return BadRequest("Password must be at least 6 characters.");

            var user = _unitOfWork.Users.GetById(id);
            if (user == null)
                return NotFound("User not found");

            // 🔒 Security: ممنوع تغيير باسورد أي Admin تاني
            if (user.Role == "Admin")
                return BadRequest("Cannot reset password for Admin accounts. Please contact system support.");

            // ✅ تشفير الباسورد قبل الحفظ
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            _unitOfWork.Users.Update(user);
            _unitOfWork.SaveChanges();

            return Ok(new { message = "Password reset successfully!" });
        }
    }
}