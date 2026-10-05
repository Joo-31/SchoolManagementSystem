using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagementAPI.Models
{
    public class Class : IComparable<Class>
    {
        #region properties
        public int Id { get; set; }
        private string _name = string.Empty;
        private int _gradeId;
        private int _classteacherId;
        public Grade? Grade { get; set; }              // ← موجود
        public Teacher? ClassTeacher { get; set; }     // ← موجود

        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Name cannot be null or empty.");
                }
                _name = value;
            }
        }
        public int GradeId { get { return _gradeId; } 
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("GradeId must be greater than zero.");
                }
                _gradeId = value;
            }
        }
        public int ClassTeacherId { get { return _classteacherId; } 
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("ClassTeacherId must be greater than zero.");
                }
                _classteacherId = value;
            }
        }
        #endregion

        #region ctors
        public Class()
        {
            
        }

        public Class(int id, string name, int gradeId, int classteacherId)
        {
            Id = id;
            Name = name;
            GradeId = gradeId;
            ClassTeacherId = classteacherId;
        }

        #endregion

        #region methods

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, GradeId: {GradeId}, ClassTeacherId: {ClassTeacherId}";
        }
        public int CompareTo(Class? other)
        {
            return other is null ? 1 : Name.CompareTo(other.Name);
        }
        #endregion
    }
}
