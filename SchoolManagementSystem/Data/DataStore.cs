using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolManagementSystem.Models;
namespace SchoolManagementSystem.Data
{
    public static class DataStore
    {

        public static List<Student> Students { get; set; } = new List<Student>();
        public static List<Course> Courses { get; set; } = new List<Course>();
        public static List<Grade> Grades { get; set; } = new List<Grade>();

        public static List<Teacher> Teachers { get; set; } = new List<Teacher>();

        public static List<Attendance> Attendances { get; set; } = new List<Attendance>();

        public static List<Class> Classes { get; set; } = new List<Class>();
        

    }
}
