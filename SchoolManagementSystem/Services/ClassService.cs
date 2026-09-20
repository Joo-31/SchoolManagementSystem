using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.DataBase;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Services
{
    public class ClassService
    {
        private readonly SchoolDbContext _context;

        public ClassService()
        {
            _context = new SchoolDbContext();
        }

        #region CRUD
        public void Add(Class @class)
        {
            _context.Classes.Add(@class);
            _context.SaveChanges();
        }

        public List<Class> GetAll()
        {
            return _context.Classes.ToList();
        }

        public Class? GetById(int id)
        {
            return _context.Classes.FirstOrDefault(c => c.Id == id);
        }

        public bool Update(Class @class)
        {
            var existing = GetById(@class.Id);
            if (existing == null)
                return false;

            bool hasChanges =
                existing.Name != @class.Name ||
                existing.GradeId != @class.GradeId ||
                existing.ClassTeacherId != @class.ClassTeacherId;

            if (!hasChanges)
                return false;

            existing.Name = @class.Name;
            existing.GradeId = @class.GradeId;
            existing.ClassTeacherId = @class.ClassTeacherId;

            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var @class = GetById(id);
            if (@class != null)
            {
                _context.Classes.Remove(@class);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region Search & Filter
        public List<Class> Search(string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return GetAll();

            var lowerKeyword = keyword.ToLower();

            return _context.Classes
                .Where(c => c.Name.ToLower().Contains(lowerKeyword))
                .ToList();
        }

        public List<Class> GetClassesOrderedByName()
        {
            return _context.Classes
                .OrderBy(c => c.Name)
                .ToList();
        }

        public List<Class> GetClassesByTeacherId(int teacherId)
        {
            return _context.Classes
                .Where(c => c.ClassTeacherId == teacherId)
                .ToList();
        }
        #endregion

        #region Statistics
        public Dictionary<int, int> GetClassCountByGrade()
        {
            return _context.Classes
                .GroupBy(c => c.GradeId)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public Dictionary<string, int> GetStudentCountByClass()
        {
            return _context.Classes
                .ToDictionary(
                    c => c.Name,
                    c => _context.Students.Count(s => s.ClassId == c.Id)
                );
        }

        public Class? GetClassWithMostStudents()
        {
            return _context.Classes
                .AsEnumerable()
                .OrderByDescending(c => _context.Students.Count(s => s.ClassId == c.Id))
                .FirstOrDefault();
        }

        public Class? GetClassWithLeastStudents()
        {
            return _context.Classes
                .AsEnumerable()
                .OrderBy(c => _context.Students.Count(s => s.ClassId == c.Id))
                .FirstOrDefault();
        }
        #endregion
    }
}