using SchoolManagementAPI.Models;

namespace SchoolManagementAPI.Services.Interfaces
{
    public interface IAuthService
    {
        bool Register(string username, string password, string role = "Student", int? teacherId = null, int? studentId = null);
        string? Login(string username, string password);

        User? GetUserByUsername(string username);
    }
}