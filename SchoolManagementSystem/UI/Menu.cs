using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Services;
using SchoolManagementSystem.UI;
namespace SchoolManagementSystem.UI
{
    public class Menu
    {

        public readonly StudentService _studentService;
        public readonly TeacherService _teacherService;
        public readonly CourseService _courseService;
        public readonly GradeService _gradeService;
        public readonly ClassService _classService;
        public readonly AttendanceService _attendanceService;

        public Menu()
        {
            _studentService = new StudentService();
            _teacherService = new TeacherService();
            _courseService = new CourseService();
            _gradeService = new GradeService();
            _classService = new ClassService();
            _attendanceService = new AttendanceService();
        }

        public void ShowMainMenu()
        {
            while (true)
            {
                Console.WriteLine("=======================================================");
                Console.WriteLine("Welcome to the School Management System");
                Console.WriteLine("=======================================================");
                Console.WriteLine("1. Manage Students");
                Console.WriteLine("2. Manage Teachers");
                Console.WriteLine("3. Manage Courses");
                Console.WriteLine("4. Manage Grades");
                Console.WriteLine("5. Manage Classes");
                Console.WriteLine("6. Manage Attendance");
                Console.WriteLine("0. Exit");
                Console.WriteLine("=======================================================");
                Console.WriteLine("choose an option: ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        ShowStudentMenu();
                        break;
                    case "2":
                        ShowTeacherMenu();
                        break;
                    case "3":
                        ShowCourseMenu();
                        break;
                    case "4":
                        ShowGradeMenu();
                        break;
                    case "5":
                        ShowClassMenu();
                        break;
                    case "6":
                        ShowAttendanceMenu();
                        break;
                    case "0":
                        Console.WriteLine("Exiting the application. Goodbye!");
                        Console.ReadKey();
                        return;
                    default:
                        Console.WriteLine("Invalid option. Press any key to try again...");
                        Console.ReadKey();
                        break;

                }
            }

        }

        private void ShowStudentMenu()
        {
            while (true)
            {

                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine("           STUDENT MANAGEMENT");
                Console.WriteLine("==========================================");
                Console.WriteLine("1. Add Student");
                Console.WriteLine("2. View All Students");
                Console.WriteLine("3. Search Student");
                Console.WriteLine("4. Update Student");
                Console.WriteLine("5. Delete Student");
                Console.WriteLine("0. Back to Main Menu");
                Console.WriteLine("==========================================");
                Console.Write("Choose an option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddStudent();
                        break;
                    case "2":
                        ViewAllStudents();
                        break;
                    case "3":
                        SearchStudent();
                        break;
                    case "4":
                        UpdateStudent();
                        break;
                    case "5":
                        DeleteStudent();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }


            }
        }
        #region Student
        private void AddStudent()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           ADD NEW STUDENT");
            Console.WriteLine("==========================================");
            Console.Write("Enter First Name: ");
            string firstName = Console.ReadLine();

            Console.Write("Enter Last Name: ");
            string lastName = Console.ReadLine();

            Console.Write("Enter Birth Year: ");
            int year = int.Parse(Console.ReadLine());

            Console.Write("Enter Birth Month: ");
            int month = int.Parse(Console.ReadLine());

            Console.Write("Enter Birth Day: ");
            int day = int.Parse(Console.ReadLine());

            DateTime birthDate = new DateTime(year, month, day);
            Console.Write("Enter Gender (Male/Female): ");
            GenderEnum gender = (GenderEnum)Enum.Parse(typeof(GenderEnum), Console.ReadLine(), true);

            Console.Write("Enter Class ID: ");
            int classId = int.Parse(Console.ReadLine());

            var student = new Student
            {
                FirstName = firstName,
                LastName = lastName,
                BirthDate = birthDate,
                Gender = gender,
                ClassId = classId
            };
            _studentService.Add(student);
            Console.WriteLine("\n Student added successfully!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        private void ViewAllStudents()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           ALL STUDENTS");
            Console.WriteLine("==========================================");
            var students = _studentService.GetAll();
            if (students.Count == 0)
            {
                Console.WriteLine("No students found.");
            }
            else
            {
                foreach (var student in students)
                {
                    Console.WriteLine(student);


                }
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void SearchStudent()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           SEARCH STUDENT");
            Console.WriteLine("==========================================");
            Console.Write("Enter keyword (First Name or Last Name): ");
            string keyword = Console.ReadLine();
            var students = _studentService.Search(keyword);
            if (students.Count == 0)
            {
                Console.WriteLine("No students found.");
            }
            else
            {
                foreach (var student in students)
                {
                    Console.WriteLine(student);
                }
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
        private void UpdateStudent()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           UPDATE STUDENT");
            Console.WriteLine("==========================================");
            Console.WriteLine("1. Search by ID");
            Console.WriteLine("2. Search by Name");
            Console.WriteLine("0. Back");
            Console.Write("Choose: ");

            var choice = Console.ReadLine();

            Student? student = null;

            switch (choice)
            {
                case "1":
                    student = FindStudentById();
                    break;
                case "2":
                    student = FindStudentByName();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    Console.ReadKey();
                    return;
            }

            if (student == null)
            {
                Console.WriteLine("\n Student not found!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           UPDATE STUDENT");
            Console.WriteLine("==========================================");
            Console.WriteLine($"Updating: {student.FullName} (ID: {student.Id})");
            Console.WriteLine("Press Enter to keep current value.\n");

            Console.Write($"First Name ({student.FirstName}): ");
            string firstName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(firstName))
                student.FirstName = firstName;

            Console.Write($"Last Name ({student.LastName}): ");
            string lastName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(lastName))
                student.LastName = lastName;

            Console.Write($"Birth Date ({student.BirthDate.ToShortDateString()}): ");
            string birthDateInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(birthDateInput) && DateTime.TryParse(birthDateInput, out DateTime birthDate))
                student.BirthDate = birthDate;

            Console.Write($"Gender ({student.Gender}): ");
            string genderInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(genderInput) && Enum.TryParse<GenderEnum>(genderInput, true, out GenderEnum gender))
                student.Gender = gender;

            Console.Write($"Class ID ({student.ClassId}): ");
            string classIdInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(classIdInput) && int.TryParse(classIdInput, out int classId))
                student.ClassId = classId;

            bool updated = _studentService.Update(student);
            if (updated)
            {
                Console.WriteLine("\n Student updated successfully!");
            }
            else
            {
                Console.WriteLine("\n No changes were made.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private Student? FindStudentById()
        {
            Console.Write("Enter Student ID: ");
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out int id))
            {
                Console.WriteLine("❌ Invalid ID.");
                return null;
            }

            return _studentService.GetById(id);
        }

        private Student? FindStudentByName()
        {
            Console.Write("Enter Student Name: ");
            string keyword = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(keyword))
                return null;

            var results = _studentService.Search(keyword);

            if (results.Count == 0)
                return null;

            if (results.Count == 1)
                return results[0];


            Console.WriteLine("\nMultiple results found:");
            for (int i = 0; i < results.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {results[i].FullName} (ID: {results[i].Id})");
            }
            Console.Write("Choose number: ");

            if (int.TryParse(Console.ReadLine(), out int index) &&
                index >= 1 && index <= results.Count)
            {
                return results[index - 1];
            }

            return null;
        }
        private void DeleteStudent()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           DELETE STUDENT");
            Console.WriteLine("==========================================");
            Console.WriteLine("1. Delete by ID");
            Console.WriteLine("2. Delete by Name");
            Console.WriteLine("0. Back");
            Console.Write("Choose: ");

            var choice = Console.ReadLine();

            Student? student = null;

            switch (choice)
            {
                case "1":
                    student = FindStudentById();
                    break;
                case "2":
                    student = FindStudentByName();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    Console.ReadKey();
                    return;
            }

            if (student == null)
            {
                Console.WriteLine("\n Student not found!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\nAre you sure you want to delete {student.FullName} (ID: {student.Id})? (y/n)");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                _studentService.Delete(student.Id);
                Console.WriteLine("\n Student deleted successfully!");
            }
            else
            {
                Console.WriteLine("\n Deletion cancelled.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        #endregion

        #region Teacher Menu
        private void ShowTeacherMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine("           TEACHER MANAGEMENT");
                Console.WriteLine("==========================================");
                Console.WriteLine("1. Add Teacher");
                Console.WriteLine("2. View All Teachers");
                Console.WriteLine("3. Search Teacher");
                Console.WriteLine("4. Update Teacher");
                Console.WriteLine("5. Delete Teacher");
                Console.WriteLine("0. Back to Main Menu");
                Console.WriteLine("==========================================");
                Console.Write("Choose an option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": AddTeacher(); break;
                    case "2": ViewAllTeachers(); break;
                    case "3": SearchTeacher(); break;
                    case "4": UpdateTeacher(); break;
                    case "5": DeleteTeacher(); break;
                    case "0": return;
                    default:
                        Console.WriteLine("Invalid option. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void AddTeacher()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           ADD NEW TEACHER");
            Console.WriteLine("==========================================");

            Console.Write("Enter Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter Email: ");
            string email = Console.ReadLine();

            Console.Write("Enter Phone: ");
            string phone = Console.ReadLine();

            Console.Write("Enter Specialization: ");
            string specialization = Console.ReadLine();

            Console.Write("Enter Hire Year: ");
            int year = int.Parse(Console.ReadLine());

            Console.Write("Enter Hire Month: ");
            int month = int.Parse(Console.ReadLine());

            Console.Write("Enter Hire Day: ");
            int day = int.Parse(Console.ReadLine());

            DateTime hireDate = new DateTime(year, month, day);

            var teacher = new Teacher
            {
                Name = name,
                Email = email,
                Phone = phone,
                Specialization = specialization,
                HireDate = hireDate
            };

            _teacherService.Add(teacher);
            Console.WriteLine("\n Teacher added successfully!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private void ViewAllTeachers()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           ALL TEACHERS");
            Console.WriteLine("==========================================");

            var teachers = _teacherService.GetAll();
            if (teachers.Count == 0)
            {
                Console.WriteLine("No teachers found.");
            }
            else
            {
                foreach (var t in teachers)
                {
                    Console.WriteLine(t);
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void SearchTeacher()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           SEARCH TEACHER");
            Console.WriteLine("==========================================");

            Console.Write("Enter search keyword (Name or Specialization): ");
            string keyword = Console.ReadLine();

            var results = _teacherService.Search(keyword);
            if (results.Count == 0)
            {
                Console.WriteLine("No teachers found.");
            }
            else
            {
                foreach (var t in results)
                {
                    Console.WriteLine(t);
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void UpdateTeacher()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           UPDATE TEACHER");
            Console.WriteLine("==========================================");
            Console.WriteLine("1. Search by ID");
            Console.WriteLine("2. Search by Name");
            Console.WriteLine("0. Back");
            Console.Write("Choose: ");

            var choice = Console.ReadLine();

            Teacher? teacher = null;

            switch (choice)
            {
                case "1": teacher = FindTeacherById(); break;
                case "2": teacher = FindTeacherByName(); break;
                case "0": return;
                default:
                    Console.WriteLine("Invalid option.");
                    Console.ReadKey();
                    return;
            }

            if (teacher == null)
            {
                Console.WriteLine("\n❌ Teacher not found!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           UPDATE TEACHER");
            Console.WriteLine("==========================================");
            Console.WriteLine($"Updating: {teacher.Name} (ID: {teacher.Id})");
            Console.WriteLine("Press Enter to keep current value.\n");

            Console.Write($"Name ({teacher.Name}): ");
            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name)) teacher.Name = name;

            Console.Write($"Email ({teacher.Email}): ");
            string email = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(email)) teacher.Email = email;

            Console.Write($"Phone ({teacher.Phone}): ");
            string phone = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(phone)) teacher.Phone = phone;

            Console.Write($"Specialization ({teacher.Specialization}): ");
            string specialization = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(specialization)) teacher.Specialization = specialization;

            var hireDate = GetDateInput($"Hire Date (current: {teacher.HireDate.ToShortDateString()}):");
            if (hireDate != null) teacher.HireDate = hireDate.Value;

            bool updated = _teacherService.Update(teacher);
            if (updated)
            {
                Console.WriteLine("\n✅ Teacher updated successfully!");
            }
            else
            {
                Console.WriteLine("\n⚠️ No changes were made.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        private void DeleteTeacher()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           DELETE TEACHER");
            Console.WriteLine("==========================================");
            Console.WriteLine("1. Delete by ID");
            Console.WriteLine("2. Delete by Name");
            Console.WriteLine("0. Back");
            Console.Write("Choose: ");

            var choice = Console.ReadLine();

            Teacher? teacher = null;

            switch (choice)
            {
                case "1": teacher = FindTeacherById(); break;
                case "2": teacher = FindTeacherByName(); break;
                case "0": return;
                default:
                    Console.WriteLine("Invalid option.");
                    Console.ReadKey();
                    return;
            }

            if (teacher == null)
            {
                Console.WriteLine("\n Teacher not found!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\nAre you sure you want to delete {teacher.Name} (ID: {teacher.Id})? (y/n)");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                _teacherService.Delete(teacher.Id);
                Console.WriteLine("\n Teacher deleted successfully!");
            }
            else
            {
                Console.WriteLine("\n Deletion cancelled.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        private Teacher? FindTeacherById()
        {
            var id = GetIntInput("Enter Teacher ID: ");
            if (id == null) return null;
            return _teacherService.GetById(id.Value);
        }

        private Teacher? FindTeacherByName()
        {
            Console.Write("Enter Teacher Name: ");
            string keyword = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(keyword)) return null;

            var results = _teacherService.Search(keyword);
            if (results.Count == 0) return null;
            if (results.Count == 1) return results[0];

            Console.WriteLine("\nMultiple results found:");
            for (int i = 0; i < results.Count; i++)
                Console.WriteLine($"{i + 1}. {results[i].Name} (ID: {results[i].Id})");
            Console.Write("Choose number: ");

            if (int.TryParse(Console.ReadLine(), out int index) &&
                index >= 1 && index <= results.Count)
                return results[index - 1];

            return null;
        }
        #endregion

        #region Course Menu
        private void ShowCourseMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine("           COURSE MANAGEMENT");
                Console.WriteLine("==========================================");
                Console.WriteLine("1. Add Course");
                Console.WriteLine("2. View All Courses");
                Console.WriteLine("3. Search Course");
                Console.WriteLine("4. Update Course");
                Console.WriteLine("5. Delete Course");
                Console.WriteLine("0. Back to Main Menu");
                Console.WriteLine("==========================================");
                Console.Write("Choose an option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": AddCourse(); break;
                    case "2": ViewAllCourses(); break;
                    case "3": SearchCourse(); break;
                    case "4": UpdateCourse(); break;
                    case "5": DeleteCourse(); break;
                    case "0": return;
                    default:
                        Console.WriteLine("Invalid option. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void AddCourse()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           ADD NEW COURSE");
            Console.WriteLine("==========================================");

            Console.Write("Enter Course Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter Course Code: ");
            string code = Console.ReadLine();

            Console.Write("Enter Credits: ");
            int credits = int.Parse(Console.ReadLine());

            Console.Write("Enter Grade ID: ");
            int gradeId = int.Parse(Console.ReadLine());

            var course = new Course
            {
                Name = name,
                Code = code,
                Credits = credits,
                GradeId = gradeId
            };

            _courseService.Add(course);
            Console.WriteLine("\n Course added successfully!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private void ViewAllCourses()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           ALL COURSES");
            Console.WriteLine("==========================================");

            var courses = _courseService.GetAll();
            if (courses.Count == 0)
            {
                Console.WriteLine("No courses found.");
            }
            else
            {
                foreach (var c in courses)
                {
                    Console.WriteLine(c);
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void SearchCourse()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           SEARCH COURSE");
            Console.WriteLine("==========================================");
            Console.Write("Enter search keyword (Name or Code): ");
            string keyword = Console.ReadLine();

            var results = _courseService.Search(keyword);
            if (results.Count == 0)
            {
                Console.WriteLine("No courses found.");
            }
            else
            {
                foreach (var c in results)
                {
                    Console.WriteLine(c);
                }
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void UpdateCourse()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           UPDATE COURSE");
            Console.WriteLine("==========================================");
            Console.WriteLine("1. Search by ID");
            Console.WriteLine("2. Search by Name");
            Console.WriteLine("0. Back");
            Console.Write("Choose: ");

            var choice = Console.ReadLine();

            Course? course = null;

            switch (choice)
            {
                case "1": course = FindCourseById(); break;
                case "2": course = FindCourseByName(); break;
                case "0": return;
                default:
                    Console.WriteLine("Invalid option.");
                    Console.ReadKey();
                    return;
            }

            if (course == null)
            {
                Console.WriteLine("\n Course not found!");
                Console.ReadKey();
                return;
            }

            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           UPDATE COURSE");
            Console.WriteLine("==========================================");
            Console.WriteLine($"Updating: {course.Name} (ID: {course.Id})");
            Console.WriteLine("Press Enter to keep current value.\n");

            Console.Write($"Name ({course.Name}): ");
            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name)) course.Name = name;

            Console.Write($"Code ({course.Code}): ");
            string code = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(code)) course.Code = code;

            Console.Write($"Credits ({course.Credits}): ");
            string creditsInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(creditsInput) && int.TryParse(creditsInput, out int credits))
                course.Credits = credits;

            Console.Write($"Grade ID ({course.GradeId}): ");
            string gradeIdInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(gradeIdInput) && int.TryParse(gradeIdInput, out int gradeId))
                course.GradeId = gradeId;

            bool updated = _courseService.Update(course);
            if (updated)
                Console.WriteLine("\n Course updated successfully!");
            else
                Console.WriteLine("\n No changes were made.");

            Console.ReadKey();
        }

        private void DeleteCourse()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           DELETE COURSE");
            Console.WriteLine("==========================================");
            Console.WriteLine("1. Delete by ID");
            Console.WriteLine("2. Delete by Name");
            Console.WriteLine("0. Back");
            Console.Write("Choose: ");

            var choice = Console.ReadLine();

            Course? course = null;

            switch (choice)
            {
                case "1": course = FindCourseById(); break;
                case "2": course = FindCourseByName(); break;
                case "0": return;
                default:
                    Console.WriteLine("Invalid option.");
                    Console.ReadKey();
                    return;
            }

            if (course == null)
            {
                Console.WriteLine("\n Course not found!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\nAre you sure you want to delete {course.Name} (Code: {course.Code})? (y/n)");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                _courseService.Delete(course.Id);
                Console.WriteLine("\n Course deleted successfully!");
            }
            else
            {
                Console.WriteLine("\n Deletion cancelled.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        private Course? FindCourseById()
        {
            var id = GetIntInput("Enter Course ID: ");
            if (id == null) return null;
            return _courseService.GetById(id.Value);
        }

        private Course? FindCourseByName()
        {
            Console.Write("Enter Course Name: ");
            string keyword = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(keyword)) return null;

            var results = _courseService.Search(keyword);
            if (results.Count == 0) return null;
            if (results.Count == 1) return results[0];

            Console.WriteLine("\nMultiple results found:");
            for (int i = 0; i < results.Count; i++)
                Console.WriteLine($"{i + 1}. {results[i].Name} (ID: {results[i].Id})");
            Console.Write("Choose number: ");

            if (int.TryParse(Console.ReadLine(), out int index) &&
                index >= 1 && index <= results.Count)
                return results[index - 1];

            return null;
        }
        #endregion

        #region Grade Menu
        private void ShowGradeMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine("           GRADE MANAGEMENT");
                Console.WriteLine("==========================================");
                Console.WriteLine("1. Add Grade");
                Console.WriteLine("2. View All Grades");
                Console.WriteLine("3. Search Grade");
                Console.WriteLine("4. Update Grade");
                Console.WriteLine("5. Delete Grade");
                Console.WriteLine("0. Back to Main Menu");
                Console.WriteLine("==========================================");
                Console.Write("Choose an option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": AddGrade(); break;
                    case "2": ViewAllGrades(); break;
                    case "3": SearchGrade(); break;
                    case "4": UpdateGrade(); break;
                    case "5": DeleteGrade(); break;
                    case "0": return;
                    default:
                        Console.WriteLine("Invalid option. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void AddGrade()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           ADD NEW GRADE");
            Console.WriteLine("==========================================");

            Console.Write("Enter Grade Name (e.g., Primary, Secondary): ");
            string name = Console.ReadLine();

            Console.Write("Enter Level (1, 2, 3): ");
            int level = int.Parse(Console.ReadLine());

            var grade = new Grade
            {
                Name = name,
                Level = level
            };

            _gradeService.Add(grade);
            Console.WriteLine("\n Grade added successfully!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private void ViewAllGrades()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           ALL GRADES");
            Console.WriteLine("==========================================");

            var grades = _gradeService.GetAll();
            if (grades.Count == 0)
            {
                Console.WriteLine("No grades found.");
            }
            else
            {
                foreach (var g in grades)
                {
                    Console.WriteLine(g);
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void SearchGrade()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           SEARCH GRADE");
            Console.WriteLine("==========================================");

            Console.Write("Enter search keyword (Name): ");
            string keyword = Console.ReadLine();

            var results = _gradeService.Search(keyword);
            if (results.Count == 0)
            {
                Console.WriteLine("No grades found.");
            }
            else
            {
                foreach (var g in results)
                {
                    Console.WriteLine(g);
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void UpdateGrade()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           UPDATE GRADE");
            Console.WriteLine("==========================================");
            Console.WriteLine("1. Search by ID");
            Console.WriteLine("2. Search by Name");
            Console.WriteLine("0. Back");
            Console.Write("Choose: ");

            var choice = Console.ReadLine();

            Grade? grade = null;  // ← تعريف واحد بس

            switch (choice)
            {
                case "1": grade = FindGradeById(); break;
                case "2": grade = FindGradeByName(); break;
                case "0": return;
                default:
                    Console.WriteLine("Invalid option.");
                    Console.ReadKey();
                    return;
            }

            if (grade == null)
            {
                Console.WriteLine("\n Grade not found!");
                Console.ReadKey();
                return;
            }

            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           UPDATE GRADE");
            Console.WriteLine("==========================================");
            Console.WriteLine($"Updating: {grade.Name} (ID: {grade.Id})");
            Console.WriteLine("Press Enter to keep current value.\n");

            Console.Write($"Name ({grade.Name}): ");
            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name)) grade.Name = name;

            Console.Write($"Level ({grade.Level}): ");
            string levelInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(levelInput) && int.TryParse(levelInput, out int level))
                grade.Level = level;

            bool updated = _gradeService.Update(grade);
            if (updated)
                Console.WriteLine("\n Grade updated successfully!");
            else
                Console.WriteLine("\n No changes were made.");

            Console.ReadKey();
        }
        private void DeleteGrade()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           DELETE GRADE");
            Console.WriteLine("==========================================");
            Console.WriteLine("1. Delete by ID");
            Console.WriteLine("2. Delete by Name");
            Console.WriteLine("0. Back");
            Console.Write("Choose: ");

            var choice = Console.ReadLine();

            Grade? grade = null;

            switch (choice)
            {
                case "1": grade = FindGradeById(); break;
                case "2": grade = FindGradeByName(); break;
                case "0": return;
                default:
                    Console.WriteLine("Invalid option.");
                    Console.ReadKey();
                    return;
            }

            if (grade == null)
            {
                Console.WriteLine("\n❌ Grade not found!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\nAre you sure you want to delete {grade.Name} (Level: {grade.Level})? (y/n)");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                _gradeService.Delete(grade.Id);
                Console.WriteLine("\n✅ Grade deleted successfully!");
            }
            else
            {
                Console.WriteLine("\n❌ Deletion cancelled.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        private Grade? FindGradeById()
        {
            var id = GetIntInput("Enter Grade ID: ");
            if (id == null) return null;
            return _gradeService.GetById(id.Value);
        }

        private Grade? FindGradeByName()
        {
            Console.Write("Enter Grade Name: ");
            string keyword = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(keyword)) return null;

            var results = _gradeService.Search(keyword);
            if (results.Count == 0) return null;
            if (results.Count == 1) return results[0];

            Console.WriteLine("\nMultiple results found:");
            for (int i = 0; i < results.Count; i++)
                Console.WriteLine($"{i + 1}. {results[i].Name} (ID: {results[i].Id})");
            Console.Write("Choose number: ");

            if (int.TryParse(Console.ReadLine(), out int index) &&
                index >= 1 && index <= results.Count)
                return results[index - 1];

            return null;
        }
        #endregion

        #region Class Menu
        private void ShowClassMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine("           CLASS MANAGEMENT");
                Console.WriteLine("==========================================");
                Console.WriteLine("1. Add Class");
                Console.WriteLine("2. View All Classes");
                Console.WriteLine("3. Search Class");
                Console.WriteLine("4. Update Class");
                Console.WriteLine("5. Delete Class");
                Console.WriteLine("0. Back to Main Menu");
                Console.WriteLine("==========================================");
                Console.Write("Choose an option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": AddClass(); break;
                    case "2": ViewAllClasses(); break;
                    case "3": SearchClass(); break;
                    case "4": UpdateClass(); break;
                    case "5": DeleteClass(); break;
                    case "0": return;
                    default:
                        Console.WriteLine("Invalid option. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void AddClass()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           ADD NEW CLASS");
            Console.WriteLine("==========================================");

            Console.Write("Enter Class Name (e.g., 3A, 5B): ");
            string name = Console.ReadLine();

            Console.Write("Enter Grade ID: ");
            int gradeId = int.Parse(Console.ReadLine());

            Console.Write("Enter Class Teacher ID: ");
            int teacherId = int.Parse(Console.ReadLine());

            var classObj = new Class
            {
                Name = name,
                GradeId = gradeId,
                ClassTeacherId = teacherId
            };

            _classService.Add(classObj);
            Console.WriteLine("\n✅ Class added successfully!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private void ViewAllClasses()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           ALL CLASSES");
            Console.WriteLine("==========================================");

            var classes = _classService.GetAll();
            if (classes.Count == 0)
            {
                Console.WriteLine("No classes found.");
            }
            else
            {
                foreach (var c in classes)
                {
                    Console.WriteLine(c);
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void SearchClass()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           SEARCH CLASS");
            Console.WriteLine("==========================================");

            Console.Write("Enter search keyword (Name): ");
            string keyword = Console.ReadLine();

            var results = _classService.Search(keyword);
            if (results.Count == 0)
            {
                Console.WriteLine("No classes found.");
            }
            else
            {
                foreach (var c in results)
                {
                    Console.WriteLine(c);
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void UpdateClass()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           UPDATE CLASS");
            Console.WriteLine("==========================================");
            Console.WriteLine("1. Search by ID");
            Console.WriteLine("2. Search by Name");
            Console.WriteLine("0. Back");
            Console.Write("Choose: ");

            var choice = Console.ReadLine();

            Class? classObj = null;

            switch (choice)
            {
                case "1": classObj = FindClassById(); break;
                case "2": classObj = FindClassByName(); break;
                case "0": return;
                default:
                    Console.WriteLine("Invalid option.");
                    Console.ReadKey();
                    return;
            }

            if (classObj == null)
            {
                Console.WriteLine("\n❌ Class not found!");
                Console.ReadKey();
                return;
            }

            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           UPDATE CLASS");
            Console.WriteLine("==========================================");
            Console.WriteLine($"Updating: {classObj.Name} (ID: {classObj.Id})");
            Console.WriteLine("Press Enter to keep current value.\n");

            Console.Write($"Name ({classObj.Name}): ");
            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name)) classObj.Name = name;

            Console.Write($"Grade ID ({classObj.GradeId}): ");
            string gradeIdInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(gradeIdInput) && int.TryParse(gradeIdInput, out int gradeId))
                classObj.GradeId = gradeId;

            Console.Write($"Class Teacher ID ({classObj.ClassTeacherId}): ");
            string teacherIdInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(teacherIdInput) && int.TryParse(teacherIdInput, out int teacherId))
                classObj.ClassTeacherId = teacherId;

            bool updated = _classService.Update(classObj);
            if (updated)
                Console.WriteLine("\n Class updated successfully!");
            else
                Console.WriteLine("\n No changes were made.");

            Console.ReadKey();
        }
        private void DeleteClass()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           DELETE CLASS");
            Console.WriteLine("==========================================");
            Console.WriteLine("1. Delete by ID");
            Console.WriteLine("2. Delete by Name");
            Console.WriteLine("0. Back");
            Console.Write("Choose: ");

            var choice = Console.ReadLine();

            Class? classObj = null;

            switch (choice)
            {
                case "1": classObj = FindClassById(); break;
                case "2": classObj = FindClassByName(); break;
                case "0": return;
                default:
                    Console.WriteLine("Invalid option.");
                    Console.ReadKey();
                    return;
            }

            if (classObj == null)
            {
                Console.WriteLine("\n❌ Class not found!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\nAre you sure you want to delete {classObj.Name} (ID: {classObj.Id})? (y/n)");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                _classService.Delete(classObj.Id);
                Console.WriteLine("\n Class deleted successfully!");
            }
            else
            {
                Console.WriteLine("\n Deletion cancelled.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        private Class? FindClassById()
        {
            var id = GetIntInput("Enter Class ID: ");
            if (id == null) return null;
            return _classService.GetById(id.Value);
        }

        private Class? FindClassByName()
        {
            Console.Write("Enter Class Name: ");
            string keyword = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(keyword)) return null;

            var results = _classService.Search(keyword);
            if (results.Count == 0) return null;
            if (results.Count == 1) return results[0];

            Console.WriteLine("\nMultiple results found:");
            for (int i = 0; i < results.Count; i++)
                Console.WriteLine($"{i + 1}. {results[i].Name} (ID: {results[i].Id})");
            Console.Write("Choose number: ");

            if (int.TryParse(Console.ReadLine(), out int index) &&
                index >= 1 && index <= results.Count)
                return results[index - 1];

            return null;
        }
        #endregion

        #region Attendance Menu
        private void ShowAttendanceMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine("           ATTENDANCE MANAGEMENT");
                Console.WriteLine("==========================================");
                Console.WriteLine("1. Add Attendance");
                Console.WriteLine("2. View All Attendances");
                Console.WriteLine("3. Search by Student");
                Console.WriteLine("4. Search by Class");
                Console.WriteLine("5. Search by Date");
                Console.WriteLine("6. Update Attendance");
                Console.WriteLine("7. Delete Attendance");
                Console.WriteLine("0. Back to Main Menu");
                Console.WriteLine("==========================================");
                Console.Write("Choose an option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": AddAttendance(); break;
                    case "2": ViewAllAttendances(); break;
                    case "3": SearchAttendanceByStudent(); break;
                    case "4": SearchAttendanceByClass(); break;
                    case "5": SearchAttendanceByDate(); break;
                    case "6": UpdateAttendance(); break;
                    case "7": DeleteAttendance(); break;
                    case "0": return;
                    default:
                        Console.WriteLine("Invalid option. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void AddAttendance()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           ADD NEW ATTENDANCE");
            Console.WriteLine("==========================================");

            var studentId = GetIntInput("Enter Student ID: ");
            if (studentId == null) { Console.WriteLine("❌ Invalid ID."); Console.ReadKey(); return; }

            var classId = GetIntInput("Enter Class ID: ");
            if (classId == null) { Console.WriteLine("❌ Invalid ID."); Console.ReadKey(); return; }

            var date = GetDateInput("Enter Attendance Date:");
            if (date == null) { Console.WriteLine("❌ Invalid date."); Console.ReadKey(); return; }

            Console.Write("Is Present? (true/false): ");
            if (!bool.TryParse(Console.ReadLine(), out bool isPresent))
            {
                Console.WriteLine(" Invalid input.");
                Console.ReadKey();
                return;
            }

            var attendance = new Attendance
            {
                StudentId = studentId.Value,
                ClassId = classId.Value,
                Date = date.Value,
                IsPresent = isPresent
            };

            _attendanceService.Add(attendance);
            Console.WriteLine("\n Attendance added successfully!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        private void ViewAllAttendances()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           ALL ATTENDANCES");
            Console.WriteLine("==========================================");

            var attendances = _attendanceService.GetAll();
            if (attendances.Count == 0)
            {
                Console.WriteLine("No attendance records found.");
            }
            else
            {
                foreach (var a in attendances)
                {
                    Console.WriteLine(a);
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void SearchAttendanceByStudent()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           SEARCH ATTENDANCE BY STUDENT");
            Console.WriteLine("==========================================");

            var studentId = GetIntInput("Enter Student ID: ");
            if (studentId == null)
            {
                Console.WriteLine(" Invalid ID.");
                Console.ReadKey();
                return;
            }

            var results = _attendanceService.SearchByStudentId(studentId.Value);


            if (results.Count == 0)
            {
                Console.WriteLine("No attendance records found.");
            }
            else
            {
                foreach (var a in results)
                {
                    Console.WriteLine(a);
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void SearchAttendanceByClass()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           SEARCH ATTENDANCE BY CLASS");
            Console.WriteLine("==========================================");

            var studentId = GetIntInput("Enter Student ID: ");
            if (studentId == null)
            {
                Console.WriteLine(" Invalid ID.");
                Console.ReadKey();
                return;
            }

            var results = _attendanceService.SearchByStudentId(studentId.Value);
            if (results.Count == 0)
            {
                Console.WriteLine("No attendance records found.");
            }
            else
            {
                foreach (var a in results)
                {
                    Console.WriteLine(a);
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void SearchAttendanceByDate()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           SEARCH ATTENDANCE BY DATE");
            Console.WriteLine("==========================================");

            var date = GetDateInput("Enter Date:");
            if (date == null)
            {
                Console.WriteLine("❌ Invalid date.");
                Console.ReadKey();
                return;
            }

            var results = _attendanceService.SearchByDate(date.Value);
            if (results.Count == 0)
                Console.WriteLine("No attendance records found.");
            else
                foreach (var a in results)
                    Console.WriteLine(a);

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
        private void UpdateAttendance()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           UPDATE ATTENDANCE");
            Console.WriteLine("==========================================");

            Console.Write("Enter Attendance ID to update: ");
            int id = int.Parse(Console.ReadLine());

            var attendance = _attendanceService.GetById(id);
            if (attendance == null)
            {
                Console.WriteLine("Attendance record not found!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.Write($"Enter new Student ID ({attendance.StudentId}): ");
            string studentIdInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(studentIdInput)) attendance.StudentId = int.Parse(studentIdInput);

            Console.Write($"Enter new Class ID ({attendance.ClassId}): ");
            string classIdInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(classIdInput)) attendance.ClassId = int.Parse(classIdInput);

            var date = GetDateInput($"Enter new Date (current: {attendance.Date.ToShortDateString()}):");
            if (date != null) attendance.Date = date.Value;

            Console.Write($"Is Present? ({attendance.IsPresent}): ");
            string presentInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(presentInput)) attendance.IsPresent = bool.Parse(presentInput);

            bool updated = _attendanceService.Update(attendance);
            if (updated)
            {
                Console.WriteLine("\n Attendance updated successfully!");
            }
            else
            {
                Console.WriteLine("\n No changes were made.");
            }
            Console.ReadKey();
        }

        private void DeleteAttendance()
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("           DELETE ATTENDANCE");
            Console.WriteLine("==========================================");

            var id = GetIntInput("Enter Attendance ID to delete: ");
            if (id == null)
            {
                Console.WriteLine(" Invalid ID.");
                Console.ReadKey();
                return;
            }

            var attendance = _attendanceService.GetById(id.Value);
            if (attendance == null)
            {
                Console.WriteLine("\n❌ Attendance record not found!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\nAre you sure you want to delete this attendance record? (y/n)");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                _attendanceService.Delete(attendance.Id);
                Console.WriteLine("\n Attendance deleted successfully!");
            }
            else
            {
                Console.WriteLine("\n Deletion cancelled.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        #endregion
        private int? GetIntInput(string prompt)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out int result))
                return null;

            return result;
        }
        private DateTime? GetDateInput(string prompt)
        {
            Console.WriteLine(prompt);

            Console.Write("Enter Year (yyyy): ");
            if (!int.TryParse(Console.ReadLine(), out int year))
                return null;

            Console.Write("Enter Month (1-12): ");
            if (!int.TryParse(Console.ReadLine(), out int month))
                return null;

            Console.Write("Enter Day (1-31): ");
            if (!int.TryParse(Console.ReadLine(), out int day))
                return null;

            try
            {
                return new DateTime(year, month, day);
            }
            catch
            {
                return null;
            }
        }
    }
}


