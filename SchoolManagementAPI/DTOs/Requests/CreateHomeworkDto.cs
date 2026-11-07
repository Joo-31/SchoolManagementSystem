namespace SchoolManagementAPI.DTOs.Requests
{
    public class CreateHomeworkDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public int CourseId { get; set; }
    }
}