using System;

namespace SchoolManagementAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Student";  // Admin, Teacher, Student
        public int? TeacherId { get; set; }
        public int? StudentId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}