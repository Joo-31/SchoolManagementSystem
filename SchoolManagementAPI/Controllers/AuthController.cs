using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementAPI.Services;
using SchoolManagementAPI.Services.Interfaces;


namespace SchoolManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            try
            {
                var result = _authService.Register(request.Username, request.Password, request.Role);
                if (!result)
                    return BadRequest("Username already exists");

                return Ok("User registered successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var token = _authService.Login(request.Username, request.Password);
            if (token == null)
                return Unauthorized("Invalid username or password");

            var user = _authService.GetUserByUsername(request.Username); // ← محتاج Method جديدة

            return Ok(new
            {
                Token = token,
                Role = user?.Role,
                Username = user?.Username
            });
        }
    }

    // ✅ Models للـ Request Body
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Student";
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}