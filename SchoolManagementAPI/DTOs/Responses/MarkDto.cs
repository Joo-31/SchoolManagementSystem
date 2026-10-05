namespace SchoolManagementAPI.DTOs.Responses
{
    public class MarkDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public double Score { get; set; }
        public DateTime Date { get; set; }
        public string? Notes { get; set; }
    }
}