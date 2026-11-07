namespace SchoolManagementAPI.DTOs.Requests
{
    public class CreateNotificationDto
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string TargetRole { get; set; } = "All"; // All, Student, Teacher
    }
}