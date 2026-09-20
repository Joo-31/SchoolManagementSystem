namespace SchoolManagementAPI.DTOs.Requests
{
    public class CreateTeacherDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
    }
}