namespace SchoolManagementAPI.DTOs.Requests
{
    public class UpdateMarkDto
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int TeacherId { get; set; }
        public double Score { get; set; }
        public DateTime Date { get; set; }
        public string? Notes { get; set; }
    }
}