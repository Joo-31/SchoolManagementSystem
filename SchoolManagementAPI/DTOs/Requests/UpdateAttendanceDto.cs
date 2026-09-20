namespace SchoolManagementAPI.DTOs.Requests
{
    public class UpdateAttendanceDto
    {
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
    }
}