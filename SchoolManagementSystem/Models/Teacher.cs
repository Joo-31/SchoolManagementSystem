using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Models
{
    public class Teacher : IComparable<Teacher>
    {

        #region Properties
        public int Id { get; set; }
        private string _name=string.Empty;
        private string _email=string.Empty;
        private string _phone=string.Empty;
        private string _specialization=string.Empty;
        private DateTime _hireDate;

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
        public string Email
        {
            get { return _email; }
            set
            {
                if (string.IsNullOrWhiteSpace(value) )
                {
                    throw new ArgumentException("Email cannot be null or empty.");
                } 
                if (!value.Contains("@"))
                {
                    throw new ArgumentException("Email must be a valid email address.");
                }

                _email = value;
            }
        }
        public string Phone
        {
            get { return _phone; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Phone cannot be null or empty.");
                }
                _phone = value;
            }
        }
        public string Specialization
        {
            get { return _specialization; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Specialization cannot be null or empty.");
                }
                _specialization = value;
            }
        }

        public DateTime HireDate
        {
            get { return _hireDate; }
            set
            {
                if (value > DateTime.Now)
                {
                    throw new ArgumentException("Hire date cannot be in the future.");
                }
                _hireDate = value;
            }
        }

        #endregion
        #region ctors

        public Teacher() { }
        public Teacher(int id, string name, string email, string phone, string specialization, DateTime hireDate)
        {
            Id = id;
            Name = name;
            Email = email;
            Phone = phone;
            Specialization = specialization;
            HireDate = hireDate;
           
        }


        #endregion
        #region methods

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Email: {Email}, Phone: {Phone}, Specialization: {Specialization}, HireDate: {HireDate.ToShortDateString()}";
        }

        public int CompareTo(Teacher? other)
        {
            return other is null ? 1 : Name.CompareTo(other.Name);
        }

        public int GetYearsOfExperience()
        {

         int experience = DateTime.Now.Year - HireDate.Year;
            if (_hireDate > DateTime.Now.AddYears(-experience)) experience--;
            return experience;
        }



        #endregion
    }
}
