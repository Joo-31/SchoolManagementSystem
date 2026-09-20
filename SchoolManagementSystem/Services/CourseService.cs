using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.DataBase;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Services
{
    public class CourseService
    {
        private readonly SchoolDbContext _context;

        public CourseService()
        {
            _context = new SchoolDbContext();
        }

        #region CRUD
        public void Add(Course course)
        {
            _context.Courses.Add(course);
            _context.SaveChanges();
        }

        public List<Course> GetAll()
        {
            return _context.Courses.ToList();
        }

        public Course? GetById(int id)
        {
            return _context.Courses.FirstOrDefault(c => c.Id == id);
        }

        public bool Update(Course course)
        {
            var existing = GetById(course.Id);
            if (existing == null)
                return false;

            bool hasChanges =
                existing.Name != course.Name ||
                existing.Code != course.Code ||
                existing.Credits != course.Credits ||
                existing.GradeId != course.GradeId;

            if (!hasChanges)
                return false;

            existing.Name = course.Name;
            existing.Code = course.Code;
            existing.Credits = course.Credits;
            existing.GradeId = course.GradeId;

            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var course = GetById(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region Search & Filter
        public List<Course> Search(string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return GetAll();

            var lowerKeyword = keyword.ToLower();

            return _context.Courses
                .Where(c => c.Name.ToLower().Contains(lowerKeyword) ||
                            c.Code.ToLower().Contains(lowerKeyword))
                .ToList();
        }
        public List<Course> GetCoursesByGradeId(int gradeId)
        {
            return _context.Courses.Where(c => c.GradeId == gradeId).ToList();
        }

        public List<Course> GetCoursesOrderedByCredits()
        {
            return _context.Courses
                .OrderByDescending(c => c.Credits)
                .ToList();
        }

        public List<Course> GetCoursesWithCreditsMoreThan(int credits)
        {
            return _context.Courses
                .Where(c => c.Credits > credits)
                .ToList();
        }
        #endregion

        #region Statistics
        public Dictionary<int, int> GetCourseCountByGrade()
        {
            return _context.Courses
                .GroupBy(c => c.GradeId)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public double GetAverageCredits()
        {
            if (!_context.Courses.Any())
                return 0;

            return _context.Courses.Average(c => c.Credits);
        }

        public Course? GetCourseWithMaxCredits()
        {
            return _context.Courses
                .OrderByDescending(c => c.Credits)
                .FirstOrDefault();
        }

        public Course? GetCourseWithMinCredits()
        {
            return _context.Courses
                .OrderBy(c => c.Credits)
                .FirstOrDefault();
        }
        #endregion
    }
}