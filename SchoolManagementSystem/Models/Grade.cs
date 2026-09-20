using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Models
{
    public class Grade : IComparable<Grade>
    {
        #region Properties
        public int Id { get; set; }
        private string _name = string.Empty;
        private int _level;

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
        public int Level
        {
            get { return _level; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Level must be greater than zero.");
                }
                _level = value;
            }

        }


        #endregion

        #region Constructors

        public Grade() { }

        public Grade(int id, string name, int level)
        {
            Id = id;
            Name = name;
            Level = level;
        }


        #endregion

        #region Methods

        public override string ToString() { 
        return $"Grade Id: {Id}, Name: {Name}, Level: {Level}";
        }

        public int CompareTo(Grade? other)
        {
            return other is null ? 1 : Name.CompareTo(other.Name);
        }

        #endregion
    }
}
