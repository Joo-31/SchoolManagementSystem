namespace SchoolManagementAPI.DTOs.Responses
{
    public class AttendanceDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
    }
}