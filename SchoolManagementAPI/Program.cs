using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SchoolManagementAPI.DataBase;
using SchoolManagementAPI.Mappings;
using SchoolManagementAPI.Middlewares;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Models.Responses;
using SchoolManagementAPI.Repositories;
using SchoolManagementAPI.Repositories.Interfaces;
using SchoolManagementAPI.Services;
using SchoolManagementAPI.Services.Interfaces;
using Serilog;

namespace SchoolManagementAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // ✅ إعداد Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build())
                .CreateLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // ✅ استخدم Serilog
                builder.Host.UseSerilog();

                builder.Services.AddControllers()
                    .ConfigureApiBehaviorOptions(options =>
                    {
                        options.InvalidModelStateResponseFactory = context =>
                        {
                            var errors = context.ModelState
                                .Where(x => x.Value?.Errors.Count > 0)
                                .SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage))
                                .ToList();

                            var response = new ErrorResponse
                            {
                                StatusCode = 400,
                                Message = "Validation failed",
                                Details = string.Join(", ", errors)
                            };

                            return new BadRequestObjectResult(response);
                        };
                    });

                // ✅ تسجيل الـ Services
                builder.Services.AddScoped<IStudentService, StudentService>();
                builder.Services.AddScoped<ITeacherService, TeacherService>();
                builder.Services.AddScoped<ICourseService, CourseService>();
                builder.Services.AddScoped<IClassService, ClassService>();
                builder.Services.AddScoped<IGradeService, GradeService>();
                builder.Services.AddScoped<IAttendanceService, AttendanceService>();
                builder.Services.AddScoped<IAuthService, AuthService>();
                builder.Services.AddScoped<IMarkService, MarkService>();

                // ✅ Unit of Work
                builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

                builder.Services.AddDbContext<SchoolDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

                // ✅ JWT Authentication
                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = builder.Configuration["Jwt:Issuer"],
                            ValidAudience = builder.Configuration["Jwt:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                            ),
                            RoleClaimType = ClaimTypes.Role,
                            NameClaimType = ClaimTypes.Name,
                        };
                    });

                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(options =>
                {
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Enter your JWT token"
                    });
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                        }
                    });
                });

                builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

                // ✅ CORS
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAngular", policy =>
                    {
                        policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
                });

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }
                app.UseMiddleware<ExceptionMiddleware>();
                app.UseCors("AllowAngular");
                app.UseHttpsRedirection();

                app.UseAuthentication();
                app.UseAuthorization();

                app.MapControllers();

                // ✅ Seed Data (قبل app.Run)
                using (var scope = app.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();
                    SeedData(context);
                }

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                // ✅ إغلاق Serilog (بعد app.Run)
                Log.CloseAndFlush();
            }
        }

        // ✅ Seed Method
        // ✅ Seed Method (Stress Testing)
        static void SeedData(SchoolDbContext context)
        {
            // لو الداتابيز فيها داتا، اطلع بره عشان منعملش تكرار
            if (context.Grades.Any()) return;

            Console.WriteLine("⏳ Starting Seeding huge data... Please wait.");

            var faker = new Bogus.Faker();

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
                    HireDate = faker.Date.Past(10) // تاريخ تعيين في آخر 10 سنين
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
                    BirthDate = faker.Date.Past(18), // مواليد آخر 18 سنة
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
                    TeacherId = randomTeacher.Id
                });
            }
            context.Courses.AddRange(courses);
            context.SaveChanges();

            // 6. Attendances (50,000 سجل غياب)
            Console.WriteLine("⏳ Generating 50,000 attendances...");
            var attendances = new List<Attendance>();

            // عشان نمنع تكرار نفس الطالب في نفس اليوم، ممكن نعمل لوب ذكي
            // بس للتبسيط هنا، هنعمل 50,000 سجل عشوائي
            for (int i = 0; i < 50000; i++)
            {
                var randomStudent = faker.PickRandom(students);
                var randomClass = classes.FirstOrDefault(c => c.Id == randomStudent.ClassId) ?? faker.PickRandom(classes);

                // تواريخ عشوائية في آخر 30 يوم
                var randomDate = faker.Date.Between(DateTime.Now.AddDays(-30), DateTime.Now.AddDays(-1));

                attendances.Add(new Attendance
                {
                    StudentId = randomStudent.Id,
                    ClassId = randomClass.Id,
                    Date = randomDate,
                    IsPresent = faker.Random.Bool(0.8f) // 80% نسبة حضور
                });
            }
            context.Attendances.AddRange(attendances);
            context.SaveChanges();

            // 7. Marks (5000 علامة)
            // 7. Marks (5000 علامة فريدة)
            Console.WriteLine("⏳ Generating 5,000 unique marks...");
            var marks = new List<Mark>();
            var uniqueMarks = new HashSet<string>(); // عشان نمنع التكرار

            while (marks.Count < 5000)
            {
                var randomStudent = faker.PickRandom(students);
                var randomCourse = faker.PickRandom(courses);

                // مفتاح فريد لكل طالب وكورس
                string key = $"{randomStudent.Id}-{randomCourse.Id}";

                // لو المفتاح مش موجود، نضيف العلامة
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