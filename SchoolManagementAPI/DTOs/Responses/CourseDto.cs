namespace SchoolManagementAPI.DTOs.Responses
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int GradeId { get; set; }
        public string GradeName { get; set; } = string.Empty;
    }
}