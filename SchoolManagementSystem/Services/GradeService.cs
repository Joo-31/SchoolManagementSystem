using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.DataBase;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Services
{
    public class GradeService
    {
        private readonly SchoolDbContext _context;

        public GradeService()
        {
            _context = new SchoolDbContext();
        }

        #region CRUD
        public void Add(Grade grade)
        {
            _context.Grades.Add(grade);
            _context.SaveChanges();
        }

        public List<Grade> GetAll()
        {
            return _context.Grades.ToList();
        }

        public Grade? GetById(int id)
        {
            return _context.Grades.FirstOrDefault(g => g.Id == id);
        }

        public bool Update(Grade grade)
        {
            var existing = GetById(grade.Id);
            if (existing == null)
                return false;

            bool hasChanges =
                existing.Name != grade.Name ||
                existing.Level != grade.Level;

            if (!hasChanges)
                return false;

            existing.Name = grade.Name;
            existing.Level = grade.Level;

            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var grade = GetById(id);
            if (grade != null)
            {
                _context.Grades.Remove(grade);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region Search & Filter
        public List<Grade> Search(string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return GetAll();

            var lowerKeyword = keyword.ToLower();

            return _context.Grades
                .Where(g => g.Name.ToLower().Contains(lowerKeyword))
                .ToList();
        }

        public List<Grade> GetGradesOrderedByLevel()
        {
            return _context.Grades
                .OrderBy(g => g.Level)
                .ToList();
        }
        #endregion

        #region Statistics
        public Grade? GetHighestLevelGrade()
        {
            return _context.Grades
                .OrderByDescending(g => g.Level)
                .FirstOrDefault();
        }

        public Grade? GetLowestLevelGrade()
        {
            return _context.Grades
                .OrderBy(g => g.Level)
                .FirstOrDefault();
        }

        public Dictionary<string, int> GetClassCountByGradeName()
        {
            return _context.Grades
                .ToDictionary(
                    g => g.Name,
                    g => _context.Classes.Count(c => c.GradeId == g.Id)
                );
        }

        public Dictionary<string, int> GetCourseCountByGradeName()
        {
            return _context.Grades
                .ToDictionary(
                    g => g.Name,
                    g => _context.Courses.Count(c => c.GradeId == g.Id)
                );
        }

        public Dictionary<string, int> GetStudentCountByGradeName()
        {
            return _context.Grades
                .ToDictionary(
                    g => g.Name,
                    g => _context.Classes
                        .Where(c => c.GradeId == g.Id)
                        .Sum(c => _context.Students.Count(s => s.ClassId == c.Id))
                );
        }
        #endregion
    }
}