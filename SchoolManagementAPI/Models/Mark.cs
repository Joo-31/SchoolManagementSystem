using System;

namespace SchoolManagementAPI.Models
{
    public class Mark
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int TeacherId { get; set; }
        public double Score { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string? Notes { get; set; }

        // Navigation Properties
        public Student? Student { get; set; }
        public Course? Course { get; set; }
        public Teacher? Teacher { get; set; }
    }
}