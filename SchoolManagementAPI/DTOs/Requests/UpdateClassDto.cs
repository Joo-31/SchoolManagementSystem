namespace SchoolManagementAPI.DTOs.Requests
{
    public class UpdateClassDto
    {
        public string Name { get; set; } = string.Empty;
        public int GradeId { get; set; }
        public int ClassTeacherId { get; set; }
    }
}