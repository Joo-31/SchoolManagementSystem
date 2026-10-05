namespace SchoolManagementAPI.DTOs.Responses
{
    public class ClassDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int GradeId { get; set; }
        public int ClassTeacherId { get; set; }
        public string GradeName { get; set; } = string.Empty;
        public string ClassTeacherName { get; set; } = string.Empty;
    }
}