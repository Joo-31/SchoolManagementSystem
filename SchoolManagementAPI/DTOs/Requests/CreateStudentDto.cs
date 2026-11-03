using SchoolManagementAPI.Models;

namespace SchoolManagementAPI.DTOs.Requests
{
    public class CreateStudentDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public GenderEnum Gender { get; set; }
        public int ClassId { get; set; }

        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}