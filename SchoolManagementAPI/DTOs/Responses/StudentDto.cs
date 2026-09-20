namespace SchoolManagementAPI.DTOs.Responses
{
    public class StudentDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public int ClassId { get; set; }
    }
}