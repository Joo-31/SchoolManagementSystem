using SchoolManagementAPI.DataBase;
using SchoolManagementAPI.Repositories.Interfaces;

namespace SchoolManagementAPI.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SchoolDbContext _context;

        // ✅ Repositories (Lazy Loading)
        private IStudentRepository? _students;
        private ITeacherRepository? _teachers;
        private ICourseRepository? _courses;
        private IClassRepository? _classes;
        private IGradeRepository? _grades;
        private IAttendanceRepository? _attendances;
        private IMarkRepository? _marks;
        public UnitOfWork(SchoolDbContext context)
        {
            _context = context;
        }

        // ✅ Properties (تعمل Repository لما تحتاجها)
        public IStudentRepository Students =>
            _students ??= new StudentRepository(_context);

        public ITeacherRepository Teachers =>
            _teachers ??= new TeacherRepository(_context);

        public ICourseRepository Courses =>
            _courses ??= new CourseRepository(_context);

        public IClassRepository Classes =>
            _classes ??= new ClassRepository(_context);

        public IGradeRepository Grades =>
            _grades ??= new GradeRepository(_context);

        public IAttendanceRepository Attendances =>
            _attendances ??= new AttendanceRepository(_context);

        public IMarkRepository Marks =>
    _marks ??= new MarkRepository(_context);

        // ✅ SaveChanges
        public int SaveChanges()
        {
            var result = _context.SaveChanges();
            _context.ChangeTracker.Clear();
            return result;
        }

        // ✅ Dispose
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}