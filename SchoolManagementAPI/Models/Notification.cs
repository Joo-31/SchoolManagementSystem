using System;

namespace SchoolManagementAPI.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        // "All", "Student", "Teacher"
        public string TargetRole { get; set; } = "All";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}