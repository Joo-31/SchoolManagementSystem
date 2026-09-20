namespace SchoolManagementAPI.DTOs.Requests
{
    public class CreateGradeDto
    {
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
    }
}