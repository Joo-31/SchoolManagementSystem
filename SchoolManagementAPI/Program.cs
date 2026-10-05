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
        static void SeedData(SchoolDbContext context)
        {
            // شوف لو فيه بيانات
            if (context.Grades.Any()) return;

            // 1. Grades
            var grade1 = new Grade { Name = "Primary", Level = 1 };
            var grade2 = new Grade { Name = "Intermediate", Level = 2 };
            context.Grades.AddRange(grade1, grade2);
            context.SaveChanges();

            // 2. Teachers
            var teacher1 = new Teacher { Name = "Ahmed Mohamed", Email = "ahmed@school.com", Phone = "01001234567", Specialization = "Math", HireDate = new DateTime(2015, 9, 1) };
            var teacher2 = new Teacher { Name = "Sara Ali", Email = "sara@school.com", Phone = "01007654321", Specialization = "English", HireDate = new DateTime(2018, 9, 1) };
            var teacher3 = new Teacher { Name = "Khaled Hassan", Email = "khaled@school.com", Phone = "01009876543", Specialization = "Science", HireDate = new DateTime(2020, 9, 1) };
            context.Teachers.AddRange(teacher1, teacher2, teacher3);
            context.SaveChanges();

            // 3. Classes
            var class1 = new Class { Name = "3A", GradeId = grade1.Id, ClassTeacherId = teacher1.Id };
            var class2 = new Class { Name = "3B", GradeId = grade1.Id, ClassTeacherId = teacher1.Id };
            var class3 = new Class { Name = "5A", GradeId = grade2.Id, ClassTeacherId = teacher2.Id };
            context.Classes.AddRange(class1, class2, class3);
            context.SaveChanges();

            // 4. Students
            var students = new List<Student>
            {
                new Student { FirstName = "Omar", LastName = "Hassan", BirthDate = new DateTime(2010, 5, 15), Gender = GenderEnum.Male, ClassId = class1.Id },
                new Student { FirstName = "Laila", LastName = "Ahmed", BirthDate = new DateTime(2011, 8, 20), Gender = GenderEnum.Female, ClassId = class1.Id },
                new Student { FirstName = "Youssef", LastName = "Ali", BirthDate = new DateTime(2010, 11, 10), Gender = GenderEnum.Male, ClassId = class2.Id },
                new Student { FirstName = "Mariam", LastName = "Khaled", BirthDate = new DateTime(2009, 3, 25), Gender = GenderEnum.Female, ClassId = class2.Id },
                new Student { FirstName = "Ali", LastName = "Mahmoud", BirthDate = new DateTime(2009, 7, 12), Gender = GenderEnum.Male, ClassId = class3.Id },
                new Student { FirstName = "Nour", LastName = "Ibrahim", BirthDate = new DateTime(2008, 12, 5), Gender = GenderEnum.Female, ClassId = class3.Id },
            };
            context.Students.AddRange(students);
            context.SaveChanges();

            // 5. Courses
            var courses = new List<Course>
            {
                new Course { Name = "Mathematics", Code = "MATH101", Credits = 3, GradeId = grade1.Id },
                new Course { Name = "English", Code = "ENG101", Credits = 2, GradeId = grade1.Id },
                new Course { Name = "Science", Code = "SCI101", Credits = 3, GradeId = grade2.Id },
            };
            context.Courses.AddRange(courses);
            context.SaveChanges();

            // 6. Users (Admin + Teacher + Student)
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
                TeacherId = teacher1.Id,   
                CreatedAt = DateTime.Now
            };

            var studentUser = new User
            {
                Username = "student_omar",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123"),
                Role = "Student",
                StudentId = students[0].Id,
                CreatedAt = DateTime.Now
            };

            context.Users.AddRange(adminUser, teacherUser, studentUser);
            context.SaveChanges();
            Console.WriteLine("✅ Seed Data loaded successfully!");
        }
    }
}