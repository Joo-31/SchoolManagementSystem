using AutoMapper;
using SchoolManagementAPI.DTOs.Requests;
using SchoolManagementAPI.DTOs.Responses;
using SchoolManagementAPI.Models;

namespace SchoolManagementAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ✅ Student
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Age,
                    opt => opt.MapFrom(src => src.GetAge()))
                .ForMember(dest => dest.Gender,
                    opt => opt.MapFrom(src => src.Gender.ToString()))
                .ForMember(dest => dest.BirthDate,
        opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.ClassName,
        opt => opt.MapFrom(src => src.Class != null ? src.Class.Name : ""));

            CreateMap<CreateStudentDto, Student>();
            CreateMap<UpdateStudentDto, Student>();

            // ✅ Teacher
            CreateMap<Teacher, TeacherDto>()
                .ForMember(dest => dest.YearsOfExperience,
                    opt => opt.MapFrom(src => src.GetYearsOfExperience()));

            CreateMap<CreateTeacherDto, Teacher>();

            CreateMap<UpdateTeacherDto, Teacher>();

            // ✅ Course
            CreateMap<Course, CourseDto>()
    .ForMember(dest => dest.GradeName,
        opt => opt.MapFrom(src => src.Grade != null ? src.Grade.Name : ""));    
            CreateMap<CreateCourseDto, Course>();
            CreateMap<UpdateCourseDto, Course>();

            // ✅ Class
            CreateMap<Class, ClassDto>()
     .ForMember(dest => dest.GradeName,
         opt => opt.MapFrom(src => src.Grade != null ? src.Grade.Name : ""))
     .ForMember(dest => dest.ClassTeacherName,
         opt => opt.MapFrom(src => src.ClassTeacher != null ? src.ClassTeacher.Name : ""));
            CreateMap<CreateClassDto, Class>();
            CreateMap<UpdateClassDto, Class>();

            // ✅ Grade
            CreateMap<Grade, GradeDto>();
            CreateMap<CreateGradeDto, Grade>();
            CreateMap<UpdateGradeDto, Grade>();

            // ✅ Attendance
            CreateMap<Attendance, AttendanceDto>()
    .ForMember(dest => dest.StudentName,
        opt => opt.MapFrom(src => src.Student != null ? src.Student.FullName : ""))
    .ForMember(dest => dest.ClassName,
        opt => opt.MapFrom(src => src.Class != null ? src.Class.Name : ""));

            CreateMap<CreateAttendanceDto, Attendance>();
            CreateMap<UpdateAttendanceDto, Attendance>();
        }
    }
}