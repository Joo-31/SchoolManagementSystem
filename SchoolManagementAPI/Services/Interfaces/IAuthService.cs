using SchoolManagementAPI.Models;

namespace SchoolManagementAPI.Services.Interfaces
{
    public interface IAuthService
    {
        bool Register(string username, string password, string role = "Student");
        string? Login(string username, string password);

        User? GetUserByUsername(string username);
    }
}