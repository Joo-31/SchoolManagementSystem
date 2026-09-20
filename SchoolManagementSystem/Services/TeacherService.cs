using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.DataBase;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Services
{
    public class TeacherService
    {
        private readonly SchoolDbContext _context;

        public TeacherService()
        {
            _context = new SchoolDbContext();
        }

        #region CRUD
        public void Add(Teacher teacher)
        {
            _context.Teachers.Add(teacher);
            _context.SaveChanges();
        }

        public List<Teacher> GetAll()
        {
            return _context.Teachers.ToList();
        }

        public Teacher? GetById(int id)
        {
            return _context.Teachers.FirstOrDefault(t => t.Id == id);
        }

        public bool Update(Teacher teacher)
        {
            var existing = GetById(teacher.Id);
            if (existing == null)
                return false;

            bool hasChanges =
                existing.Name != teacher.Name ||
                existing.Email != teacher.Email ||
                existing.Phone != teacher.Phone ||
                existing.Specialization != teacher.Specialization ||
                existing.HireDate != teacher.HireDate;

            if (!hasChanges)
                return false;

            existing.Name = teacher.Name;
            existing.Email = teacher.Email;
            existing.Phone = teacher.Phone;
            existing.Specialization = teacher.Specialization;
            existing.HireDate = teacher.HireDate;

            _context.SaveChanges();
            return true;
        }
        public bool Delete(int id)
        {
            var teacher = GetById(id);
            if (teacher != null)
            {
                _context.Teachers.Remove(teacher);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region Search & Filter
        public List<Teacher> Search(string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return GetAll();

            var lowerKeyword = keyword.ToLower();

            return _context.Teachers
                .Where(t => t.Name.ToLower().Contains(lowerKeyword) ||
                            t.Specialization.ToLower().Contains(lowerKeyword))
                .ToList();
        }
        public List<Teacher> GetTeachersBySpecialization(string? specialization)
        {
            if (string.IsNullOrWhiteSpace(specialization))
                return GetAll();

            return _context.Teachers
                .Where(t => t.Specialization == specialization)
                .ToList();
        }

        public List<Teacher> GetTeachersOrderedByExperience()
        {
            return _context.Teachers
                .AsEnumerable()
                .OrderByDescending(t => t.GetYearsOfExperience())
                .ToList();
        }

        public List<Teacher> GetTeachersWithExperienceMoreThan(int years)
        {
            return _context.Teachers
                .AsEnumerable()
                .Where(t => t.GetYearsOfExperience() > years)
                .ToList();
        }
        #endregion

        #region Statistics
        public Dictionary<string, int> GetTeacherCountBySpecialization()
        {
            return _context.Teachers
                .GroupBy(t => t.Specialization)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public double GetAverageExperience()
        {
            if (!_context.Teachers.Any())
                return 0;

            return _context.Teachers.AsEnumerable().Average(t => t.GetYearsOfExperience());
        }

        public Teacher? GetNewestTeacher()
        {
            return _context.Teachers
                .OrderByDescending(t => t.HireDate)
                .FirstOrDefault();
        }

        public Teacher? GetOldestTeacher()
        {
            return _context.Teachers
                .OrderBy(t => t.HireDate)
                .FirstOrDefault();
        }

        #endregion
    }
}