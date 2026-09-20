namespace SchoolManagementAPI.DTOs.Requests
{
    public class UpdateCourseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int GradeId { get; set; }
    }
}