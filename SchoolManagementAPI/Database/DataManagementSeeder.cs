using Bogus;
using Microsoft.EntityFrameworkCore;
using SchoolManagementAPI.Models;

namespace SchoolManagementAPI.DataBase
{
    internal static class DataManagementSeeder
    {
        public static async Task<object> SeedAsync(SchoolDbContext context, CancellationToken cancellationToken)
        {
            var faker = new Faker();
            var grades = Enumerable.Range(1, 10)
                .Select(level => new Grade { Name = $"Grade {level}", Level = level }).ToList();
            context.Grades.AddRange(grades);
            await context.SaveChangesAsync(cancellationToken);

            var teachers = Enumerable.Range(0, 50).Select(_ => new Teacher
            {
                Name = faker.Name.FullName(),
                Email = faker.Internet.Email(),
                Phone = faker.Phone.PhoneNumber("010########"),
                Specialization = faker.PickRandom("Math", "Science", "English", "Arabic", "History"),
                HireDate = faker.Date.Past(10)
            }).ToList();
            context.Teachers.AddRange(teachers);
            await context.SaveChangesAsync(cancellationToken);

            var classes = Enumerable.Range(0, 100).Select(index =>
            {
                var grade = grades[index / 10];
                return new Models.Class
                {
                    Name = $"Grade {grade.Level} Class {index % 10 + 1}",
                    GradeId = grade.Id,
                    ClassTeacherId = faker.PickRandom(teachers).Id
                };
            }).ToList();
            context.Classes.AddRange(classes);
            await context.SaveChangesAsync(cancellationToken);

            var students = Enumerable.Range(0, 1000).Select(_ =>
            {
                var gender = faker.PickRandom<GenderEnum>();
                return new Student
                {
                    FirstName = faker.Name.FirstName(gender == GenderEnum.Male ? Bogus.DataSets.Name.Gender.Male : Bogus.DataSets.Name.Gender.Female),
                    LastName = faker.Name.LastName(),
                    BirthDate = faker.Date.Between(DateTime.Today.AddYears(-18), DateTime.Today.AddYears(-5)),
                    Gender = gender,
                    ClassId = faker.PickRandom(classes).Id
                };
            }).ToList();
            context.Students.AddRange(students);
            await context.SaveChangesAsync(cancellationToken);

            var courses = Enumerable.Range(1, 50).Select(index =>
            {
                var schoolClass = faker.PickRandom(classes);
                return new Course
                {
                    Name = faker.Commerce.ProductName(),
                    Code = $"CRS{index:0000}",
                    Credits = faker.Random.Int(1, 4),
                    GradeId = schoolClass.GradeId,
                    ClassId = schoolClass.Id,
                    TeacherId = faker.PickRandom(teachers).Id
                };
            }).ToList();
            context.Courses.AddRange(courses);
            await context.SaveChangesAsync(cancellationToken);

            var attendances = Enumerable.Range(0, 10_000).Select(_ =>
            {
                var student = faker.PickRandom(students);
                return new Attendance
                {
                    StudentId = student.Id,
                    ClassId = student.ClassId,
                    Date = faker.Date.Between(DateTime.Today.AddDays(-30), DateTime.Now),
                    IsPresent = faker.Random.Bool(0.8f)
                };
            }).ToList();
            context.Attendances.AddRange(attendances);
            await context.SaveChangesAsync(cancellationToken);

            var marks = new List<Mark>(5_000);
            var uniquePairs = new HashSet<(int StudentId, int CourseId)>();
            while (marks.Count < 5_000)
            {
                var student = faker.PickRandom(students);
                var course = faker.PickRandom(courses);
                if (!uniquePairs.Add((student.Id, course.Id))) continue;
                marks.Add(new Mark
                {
                    StudentId = student.Id,
                    CourseId = course.Id,
                    TeacherId = course.TeacherId,
                    Score = faker.Random.Double(0, 100),
                    Date = faker.Date.Between(DateTime.Today.AddDays(-30), DateTime.Now),
                    Notes = faker.PickRandom("Good", "Needs improvement", "Excellent", "")
                });
            }
            context.Marks.AddRange(marks);
            await context.SaveChangesAsync(cancellationToken);

            return new { grades = grades.Count, teachers = teachers.Count, classes = classes.Count, students = students.Count, courses = courses.Count, attendances = attendances.Count, marks = marks.Count };
        }
    }
}
