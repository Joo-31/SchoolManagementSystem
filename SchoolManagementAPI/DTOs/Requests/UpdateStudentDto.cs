using SchoolManagementAPI.Models;

namespace SchoolManagementAPI.DTOs.Requests
{
    public class UpdateStudentDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public GenderEnum Gender { get; set; }
        public int ClassId { get; set; }
    }
}