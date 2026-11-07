namespace SchoolManagementAPI.DTOs.Responses
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string TargetRole { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}