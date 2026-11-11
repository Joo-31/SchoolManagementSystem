using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using SchoolManagementAPI.DataBase;

namespace SchoolManagementAPI.Controllers
{
    [ApiController]
    [Route("api/admin/data")]
    [Authorize(Roles = "Admin")]
    public sealed class DataManagementController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public DataManagementController(SchoolDbContext context) => _context = context;

        [HttpPost("reset")]
        public async Task<IActionResult> Reset(CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            await _context.Homeworks.ExecuteDeleteAsync(cancellationToken);
            await _context.Notifications.ExecuteDeleteAsync(cancellationToken);
            await _context.Attendances.ExecuteDeleteAsync(cancellationToken);
            await _context.Marks.ExecuteDeleteAsync(cancellationToken);
            await _context.Users.Where(user => user.Role != "Admin").ExecuteDeleteAsync(cancellationToken);
            await _context.Users.Where(user => user.Role == "Admin")
                .ExecuteUpdateAsync(update => update
                    .SetProperty(user => user.TeacherId, (int?)null)
                    .SetProperty(user => user.StudentId, (int?)null), cancellationToken);
            await _context.Courses.ExecuteDeleteAsync(cancellationToken);
            await _context.Students.ExecuteDeleteAsync(cancellationToken);
            await _context.Classes.ExecuteDeleteAsync(cancellationToken);
            await _context.Teachers.ExecuteDeleteAsync(cancellationToken);
            await _context.Grades.ExecuteDeleteAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            return Ok(new { message = "All school data was deleted. Admin accounts were preserved." });
        }

        [HttpPost("seed")]
        public async Task<IActionResult> Seed(CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            if (await _context.Grades.AnyAsync(cancellationToken)
                || await _context.Teachers.AnyAsync(cancellationToken)
                || await _context.Students.AnyAsync(cancellationToken)
                || await _context.Courses.AnyAsync(cancellationToken)
                || await _context.Classes.AnyAsync(cancellationToken)
                || await _context.Attendances.AnyAsync(cancellationToken)
                || await _context.Marks.AnyAsync(cancellationToken)
                || await _context.Homeworks.AnyAsync(cancellationToken)
                || await _context.Notifications.AnyAsync(cancellationToken))
            {
                return Ok(new { seeded = false, message = "School data already exists; no duplicate data was created." });
            }

            var counts = await DataManagementSeeder.SeedAsync(_context, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Ok(new { seeded = true, message = "Random school data was created.", counts });
        }
    }
}
