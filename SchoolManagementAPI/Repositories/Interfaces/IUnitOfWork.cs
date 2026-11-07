namespace SchoolManagementAPI.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // ✅ كل الـ Repositories
        IStudentRepository Students { get; }
        ITeacherRepository Teachers { get; }
        ICourseRepository Courses { get; }
        IClassRepository Classes { get; }
        IGradeRepository Grades { get; }
        IAttendanceRepository Attendances { get; }

        IMarkRepository Marks { get; }

        IUserRepository Users { get; }

        INotificationRepository Notifications { get; }

        IHomeworkRepository Homeworks { get; }

        // ✅ SaveChanges واحدة للكل
        int SaveChanges();
    }
}