namespace SchoolManagementAPI.DTOs.Requests
{
    public class UpdateGradeDto
    {
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
    }
}