using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagementAPI.Models
{
    public class Attendance : IComparable<Attendance>
    {
        #region Properties
        public int Id { get; set; }
        private int _studentId { get; set; }

        private DateTime _date;
        private bool _isPresent;
        private int _classId;

        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (value > DateTime.Now)
                {
                    throw new ArgumentException("Date cannot be in the future.");
                }
                _date = value;
            }
        }
        public int ClassId
        {
            get { return _classId; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("ClassId must be greater than zero.");
                }
                _classId = value;
            }
        }
        public int StudentId
        {
            get { return _studentId; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("StudentId must be greater than zero.");
                }
                _studentId = value;
            }
        }

        public bool IsPresent
        {
            get { return _isPresent; }
            set { _isPresent = value; }
        }
        #endregion

        #region Constructors
        public Attendance() { }

        public Attendance(int id, int studentId, DateTime date, bool isPresent, int classId)
        {
            Id = id;
            StudentId = studentId;
            Date = date;
            IsPresent = isPresent;
            ClassId = classId;
        }



        #endregion

        #region Methods

        public override string ToString()
        {
            return $"Attendance Id: {Id}, StudentId: {StudentId}, Date: {Date.ToShortDateString()}, IsPresent: {IsPresent}, ClassId: {ClassId}";
        }

        public int CompareTo(Attendance? other)
        {
           return this.Date.CompareTo(other?.Date);
        }




        #endregion
    }
}
