using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagementAPI.Models
{
    public enum GenderEnum
    {
        Male,
        Female

    }
    public class Student : IComparable<Student>
    {
        #region properties
        public int Id { get; set; }
        public int ClassId { get; set; }
        private string _firstname = string.Empty;
        private string _lastname = string.Empty;
        private DateTime _birthDate;
        private GenderEnum _gender;
        public Class? Class { get; set; }   // ← Navigation Property


        public string FirstName
        {
            get { return _firstname; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Name cannot be null or empty.");
                }
                _firstname = value;
            }

        }

        public string LastName
        {
            get { return _lastname; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Name cannot be null or empty.");
                }
                _lastname = value;
            }
        }

        public GenderEnum Gender
        {
            get { return _gender; }
            set
            {
                if (value != GenderEnum.Male && value != GenderEnum.Female)
                {
                    throw new ArgumentException("Gender must be Male or Female");
                }
                _gender = value;
            }
        }

        public DateTime BirthDate
        {
            get { return _birthDate; }
            set
            {
                if (value > DateTime.Now)
                {
                    throw new ArgumentException("Birth date cannot be in the future.");
                }
                _birthDate = value;
            }
        }


        public string FullName
        {
            get { return $"{FirstName} {LastName}"; }
        }
        #endregion
        #region ctors
        public Student() { }

        public Student(int id, string firstname,string lastname, DateTime birthDate, GenderEnum gender, int classId)
        {
            Id = id;
            FirstName = firstname;
            LastName = lastname;
            BirthDate = birthDate;
            Gender = gender;
            ClassId = classId;


        }
        #endregion
        #region methods
        public override string ToString()
        {
            return $"Id: {Id}, Full Name: {FullName}, BirthDate: {BirthDate.ToShortDateString()}, Gender: {Gender}, ClassId: {ClassId}";
        }

        public int GetAge()
        {
            var today = DateTime.Today;
            var age = today.Year - _birthDate.Year;
            if (_birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }

        public int CompareTo(Student? other)
        {
            return other is null ? 1 : FullName.CompareTo(other.FullName);
        }

        #endregion


    }
}
