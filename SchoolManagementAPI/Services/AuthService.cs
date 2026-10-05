using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SchoolManagementAPI.DataBase;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Services.Interfaces;

namespace SchoolManagementAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly SchoolDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(SchoolDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ✅ تسجيل مستخدم جديد
        public bool Register(string username, string password, string role = "Student", int? teacherId = null, int? studentId = null)
        {
            // نتأكد إن الـ Username مش موجود
            if (_context.Users.Any(u => u.Username == username))
                return false;

            // نشفر كلمة السر
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                Role = role,
                TeacherId = teacherId,
                 StudentId = studentId
            };

            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }

        // ✅ تسجيل دخول
        public string? Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
                return null;

            // نتحقق من كلمة السر
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            // نعمل Token
            return GenerateToken(user);
        }

        // ✅ عمل JWT Token
        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };
            if (user.TeacherId.HasValue)
            {
                claims.Add(new Claim("TeacherId", user.TeacherId.Value.ToString()));
            }
            if (user.StudentId.HasValue)
            {
                claims.Add(new Claim("StudentId", user.StudentId.Value.ToString()));
            }
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])
                ),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public User? GetUserByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }
    }
}