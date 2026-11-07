using System;

namespace SchoolManagementAPI.Models
{
    public class Homework
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty; // رابط يوتيوب أو ملف أو كويز

        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public int TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}