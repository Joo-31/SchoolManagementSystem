namespace SchoolManagementAPI.DTOs.Responses
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        // عشان نعرف ده طالب ولا مدرس
        public int? StudentId { get; set; }
        public int? TeacherId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}