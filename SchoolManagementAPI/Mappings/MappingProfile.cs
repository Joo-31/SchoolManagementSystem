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
                    opt => opt.MapFrom(src => src.Gender.ToString()));

            CreateMap<CreateStudentDto, Student>();

            CreateMap<UpdateStudentDto, Student>();

            // ✅ Teacher
            CreateMap<Teacher, TeacherDto>()
                .ForMember(dest => dest.YearsOfExperience,
                    opt => opt.MapFrom(src => src.GetYearsOfExperience()));

            CreateMap<CreateTeacherDto, Teacher>();

            CreateMap<UpdateTeacherDto, Teacher>();

            // ✅ Course
            CreateMap<Course, CourseDto>();
            CreateMap<CreateCourseDto, Course>();
            CreateMap<UpdateCourseDto, Course>();

            // ✅ Class
            CreateMap<Class, ClassDto>();
            CreateMap<CreateClassDto, Class>();
            CreateMap<UpdateClassDto, Class>();

            // ✅ Grade
            CreateMap<Grade, GradeDto>();
            CreateMap<CreateGradeDto, Grade>();
            CreateMap<UpdateGradeDto, Grade>();

            // ✅ Attendance
            CreateMap<Attendance, AttendanceDto>();
            CreateMap<CreateAttendanceDto, Attendance>();
            CreateMap<UpdateAttendanceDto, Attendance>();
        }
    }
}