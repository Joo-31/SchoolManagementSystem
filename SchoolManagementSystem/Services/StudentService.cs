using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.DataBase;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Services
{
    public class StudentService
    {
        private readonly SchoolDbContext _context;

        public StudentService()
        {
            _context = new SchoolDbContext();
        }

        #region CRUD
        public void Add(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
        }

        public List<Student> GetAll()
        {
            return _context.Students.ToList();
        }

        public Student? GetById(int id)
        {
            return _context.Students.FirstOrDefault(s => s.Id == id);
        }

        public bool Update(Student student)
        {
            var existing = GetById(student.Id);
            if (existing == null)
                return false;

            
            bool hasChanges =
                existing.FirstName != student.FirstName ||
                existing.LastName != student.LastName ||
                existing.BirthDate != student.BirthDate ||
                existing.Gender != student.Gender ||
                existing.ClassId != student.ClassId;

            if (!hasChanges)
                return false;

            existing.FirstName = student.FirstName;
            existing.LastName = student.LastName;
            existing.BirthDate = student.BirthDate;
            existing.Gender = student.Gender;
            existing.ClassId = student.ClassId;

            _context.SaveChanges();
            return true; 
        }

        public bool Delete(int id)
        {
            var student = GetById(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region Search & Filter
        public List<Student> Search(string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return GetAll();

            var lowerKeyword = keyword.ToLower();

            return _context.Students
                .Where(s => s.FirstName.ToLower().Contains(lowerKeyword) ||
                            s.LastName.ToLower().Contains(lowerKeyword))
                .ToList();
        }

        public List<Student> GetStudentsOrderedByName()
        {
            return _context.Students
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToList();
        }

        public List<Student> GetStudentsOlderThan(int age)
        {
            return _context.Students
                .AsEnumerable()
                .Where(s => s.GetAge() > age)
                .ToList();
        }
        #endregion

        #region Statistics
        public Dictionary<int, int> GetStudentCountByClass()
        {
            return _context.Students
                .GroupBy(s => s.ClassId)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public double GetAverageAge()
        {
            if (!_context.Students.Any())
                return 0;

            return _context.Students.AsEnumerable().Average(s => s.GetAge());
        }

        public Student? GetOldestStudent()
        {
            return _context.Students
                .AsEnumerable()
                .OrderByDescending(s => s.GetAge())
                .FirstOrDefault();
        }

        public Student? GetYoungestStudent()
        {
            return _context.Students
                .AsEnumerable()
                .OrderBy(s => s.GetAge())
                .FirstOrDefault();
        }
        #endregion
    }
}