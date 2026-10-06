using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SchoolManagementAPI.Models;
namespace SchoolManagementAPI.DataBase
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options)
        {
        }
        public SchoolDbContext()
        {
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Mark> Marks { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Students → Classes
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Class)
                .WithMany()
                .HasForeignKey(s => s.ClassId)
                .OnDelete(DeleteBehavior.Restrict);   // ← بدل Cascade

            // ✅ Classes → Grades
            modelBuilder.Entity<Class>()
                .HasOne(c => c.Grade)
                .WithMany()
                .HasForeignKey(c => c.GradeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Course → Teacher
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Teacher)
                .WithMany()
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Courses → Grades
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Grade)
                .WithMany()
                .HasForeignKey(c => c.GradeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Attendances → Students
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany()
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Attendances → Classes
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Class)
                .WithMany()
                .HasForeignKey(a => a.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Marks → Students
            modelBuilder.Entity<Mark>()
                .HasOne(m => m.Student)
                .WithMany()
                .HasForeignKey(m => m.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Marks → Courses
            modelBuilder.Entity<Mark>()
                .HasOne(m => m.Course)
                .WithMany()
                .HasForeignKey(m => m.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Marks → Teachers
            modelBuilder.Entity<Mark>()
                .HasOne(m => m.Teacher)
                .WithMany()
                .HasForeignKey(m => m.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Mark Unique Constraint — (StudentId + CourseId)
            modelBuilder.Entity<Mark>()
                .HasIndex(m => new { m.StudentId, m.CourseId })
                .IsUnique();
        }







    }
}
