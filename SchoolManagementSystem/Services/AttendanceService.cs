using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.DataBase;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Services
{
    public class AttendanceService
    {
        private readonly SchoolDbContext _context;

        public AttendanceService()
        {
            _context = new SchoolDbContext();
        }

        #region CRUD
        public void Add(Attendance attendance)
        {
            _context.Attendances.Add(attendance);
            _context.SaveChanges();
        }

        public List<Attendance> GetAll()
        {
            return _context.Attendances.ToList();
        }

        public Attendance? GetById(int id)
        {
            return _context.Attendances.FirstOrDefault(a => a.Id == id);
        }

        public bool Update(Attendance attendance)
        {
            var existing = GetById(attendance.Id);
            if (existing == null)
                return false;

            bool hasChanges =
                existing.StudentId != attendance.StudentId ||
                existing.ClassId != attendance.ClassId ||
                existing.Date != attendance.Date ||
                existing.IsPresent != attendance.IsPresent;

            if (!hasChanges)
                return false;

            existing.StudentId = attendance.StudentId;
            existing.ClassId = attendance.ClassId;
            existing.Date = attendance.Date;
            existing.IsPresent = attendance.IsPresent;

            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var attendance = GetById(id);
            if (attendance != null)
            {
                _context.Attendances.Remove(attendance);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region Search & Filter
        public List<Attendance> SearchByStudentId(int studentId)
        {
            return _context.Attendances.Where(a => a.StudentId == studentId).ToList();
        }

        public List<Attendance> SearchByClassId(int classId)
        {
            return _context.Attendances.Where(a => a.ClassId == classId).ToList();
        }

        public List<Attendance> SearchByDate(DateTime date)
        {
            return _context.Attendances.Where(a => a.Date.Date == date.Date).ToList();
        }

        public List<Attendance> GetAttendanceByStudentIdAndDate(int studentId, DateTime date)
        {
            return _context.Attendances
                .Where(a => a.StudentId == studentId && a.Date.Date == date.Date)
                .ToList();
        }
        #endregion

        #region Statistics
        public int GetPresentCountByDate(DateTime date)
        {
            return _context.Attendances
                .Count(a => a.Date.Date == date.Date && a.IsPresent);
        }

        public int GetAbsentCountByDate(DateTime date)
        {
            return _context.Attendances
                .Count(a => a.Date.Date == date.Date && !a.IsPresent);
        }

        public double GetAttendancePercentageByDate(DateTime date)
        {
            var total = _context.Attendances.Count(a => a.Date.Date == date.Date);
            if (total == 0) return 0;

            var present = _context.Attendances.Count(a => a.Date.Date == date.Date && a.IsPresent);
            return (double)present / total * 100;
        }

        public int GetAbsentCountByStudent(int studentId)
        {
            return _context.Attendances
                .Count(a => a.StudentId == studentId && !a.IsPresent);
        }

        public int? GetStudentWithMostAbsences()
        {
            return _context.Attendances
                .Where(a => !a.IsPresent)
                .GroupBy(a => a.StudentId)
                .OrderByDescending(g => g.Count())
                .Select(g => (int?)g.Key)
                .FirstOrDefault();
        }

        public Dictionary<string, int> GetAttendanceCountByClass()
        {
            return _context.Classes
                .ToDictionary(
                    c => c.Name,
                    c => _context.Attendances.Count(a => a.ClassId == c.Id && a.IsPresent)
                );
        }
        #endregion
    }
}