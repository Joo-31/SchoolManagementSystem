using Bogus;
using SchoolManagementAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchoolManagementAPI.DataBase
{
    public static class DbSeeder
    {
        public static void Seed(SchoolDbContext context)
        {
            // شوف لو فيه داتا، متعملش حاجة
            if (context.Grades.Any()) return;

            Console.WriteLine("⏳ Starting Seeding huge data... Please wait.");

            var faker = new Faker();

            // 1. Grades (10 مراحل)
            var grades = new List<Grade>();
            for (int i = 1; i <= 10; i++)
            {
                grades.Add(new Grade { Name = $"Grade {i}", Level = i });
            }
            context.Grades.AddRange(grades);
            context.SaveChanges();

            // 2. Teachers (50 مدرس)
            var teachers = new List<Teacher>();
            for (int i = 0; i < 50; i++)
            {
                var teacher = new Teacher
                {
                    Name = faker.Name.FullName(),
                    Email = faker.Internet.Email(),
                    Phone = faker.Phone.PhoneNumber("010########"),
                    Specialization = faker.PickRandom(new[] { "Math", "Science", "English", "Arabic", "History" }),
                    HireDate = faker.Date.Past(10)
                };
                teachers.Add(teacher);
            }
            context.Teachers.AddRange(teachers);
            context.SaveChanges();

            // 3. Classes (30 فصل)
            var classes = new List<Class>();
            for (int i = 0; i < 30; i++)
            {
                var randomGrade = faker.PickRandom(grades);
                var randomTeacher = faker.PickRandom(teachers);

                classes.Add(new Class
                {
                    Name = $"Class {faker.Random.Number(1, 10)}{faker.Random.AlphaNumeric(1)}",
                    GradeId = randomGrade.Id,
                    ClassTeacherId = randomTeacher.Id
                });
            }
            context.Classes.AddRange(classes);
            context.SaveChanges();

            // 4. Students (1000 طالب)
            var students = new List<Student>();
            for (int i = 0; i < 1000; i++)
            {
                var randomClass = faker.PickRandom(classes);
                var gender = faker.PickRandom<GenderEnum>();

                students.Add(new Student
                {
                    FirstName = faker.Name.FirstName(gender == GenderEnum.Male ? Bogus.DataSets.Name.Gender.Male : Bogus.DataSets.Name.Gender.Female),
                    LastName = faker.Name.LastName(),
                    BirthDate = faker.Date.Past(18),
                    Gender = gender,
                    ClassId = randomClass.Id
                });
            }
            context.Students.AddRange(students);
            context.SaveChanges();

            // 5. Courses (50 كورس)
            var courses = new List<Course>();
            for (int i = 0; i < 50; i++)
            {
                var randomGrade = faker.PickRandom(grades);
                var randomTeacher = faker.PickRandom(teachers);

                courses.Add(new Course
                {
                    Name = faker.Commerce.ProductName(),
                    Code = faker.Random.AlphaNumeric(6).ToUpper(),
                    Credits = faker.Random.Number(1, 4),
                    GradeId = randomGrade.Id,
                    TeacherId = randomTeacher.Id,
                    ClassId = faker.PickRandom(classes.Where(c => c.GradeId == randomGrade.Id)).Id
                });
            }
            context.Courses.AddRange(courses);
            context.SaveChanges();

            // 6. Attendances (50,000 سجل غياب)
            Console.WriteLine("⏳ Generating 50,000 attendances...");
            var attendances = new List<Attendance>();
            for (int i = 0; i < 50000; i++)
            {
                var randomStudent = faker.PickRandom(students);
                var randomClass = classes.FirstOrDefault(c => c.Id == randomStudent.ClassId) ?? faker.PickRandom(classes);
                var randomDate = faker.Date.Between(DateTime.Now.AddDays(-30), DateTime.Now.AddDays(-1));

                attendances.Add(new Attendance
                {
                    StudentId = randomStudent.Id,
                    ClassId = randomClass.Id,
                    Date = randomDate,
                    IsPresent = faker.Random.Bool(0.8f)
                });
            }
            context.Attendances.AddRange(attendances);
            context.SaveChanges();

            // 7. Marks (5000 علامة فريدة)
            Console.WriteLine("⏳ Generating 5,000 unique marks...");
            var marks = new List<Mark>();
            var uniqueMarks = new HashSet<string>();

            while (marks.Count < 1000)
            {
                var randomStudent = faker.PickRandom(students);
                var eligibleCourses = courses.Where(c => c.ClassId == randomStudent.ClassId).ToList();
                if (eligibleCourses.Count == 0) continue;
                var randomCourse = faker.PickRandom(eligibleCourses);
                string key = $"{randomStudent.Id}-{randomCourse.Id}";

                if (uniqueMarks.Add(key))
                {
                    marks.Add(new Mark
                    {
                        StudentId = randomStudent.Id,
                        CourseId = randomCourse.Id,
                        TeacherId = randomCourse.TeacherId,
                        Score = faker.Random.Double(0, 100),
                        Date = faker.Date.Recent(30),
                        Notes = faker.Random.Bool() ? "Good" : "Needs improvement"
                    });
                }
            }
            context.Marks.AddRange(marks);
            context.SaveChanges();

            // 8. Users (Admin + Teacher + Student)
            var adminUser = new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                CreatedAt = DateTime.Now
            };

            var teacherUser = new User
            {
                Username = "teacher_ahmed",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher@123"),
                Role = "Teacher",
                TeacherId = teachers.First().Id,
                CreatedAt = DateTime.Now
            };

            var studentUser = new User
            {
                Username = "student_omar",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123"),
                Role = "Student",
                StudentId = students.First().Id,
                CreatedAt = DateTime.Now
            };

            context.Users.AddRange(adminUser, teacherUser, studentUser);
            context.SaveChanges();

            Console.WriteLine("✅ Huge Seed Data loaded successfully! 🎉");
        }
    }
}
