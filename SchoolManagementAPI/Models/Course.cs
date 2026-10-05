using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagementAPI.Models
{
    public class Course : IComparable<Course>
    {
        #region Properties
        private string _name = string.Empty;
        private string _code = string.Empty;
        private int _credits;
        public int Id { get; set; }
        public int GradeId { get; set; }
        public Grade? Grade { get; set; }   // ← Navigation Property

        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name cannot be null or empty.");
                _name = value;
            }
        }

        public string Code
        {
            get { return _code; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Code cannot be null or empty.");
                _code = value;
            }
        }

        public int Credits
        {
            get { return _credits; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Credits must be greater than zero.");
                _credits = value;
            }
        }
        #endregion

        #region Constructors
        public Course() { }

        public Course(int id, string name, string code, int credits, int gradeId)
        {
            Id = id;
            Name = name;
            Code = code;
            Credits = credits;
            GradeId = gradeId;
        }
        
        #endregion

        #region Public Methods
        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Code: {Code}, Credits: {Credits}, GradeId: {GradeId}";
        }

        public int CompareTo(Course? other)
        {
            return other is null ? 1 : Name.CompareTo(other.Name);
        }
        #endregion
    }
}
