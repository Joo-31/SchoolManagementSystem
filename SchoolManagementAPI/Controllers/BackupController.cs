using System.IO.Compression;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementAPI.DataBase;

namespace SchoolManagementAPI.Controllers;

[ApiController]
[Route("api/admin/backup")]
[Authorize(Roles = "Admin")]
public sealed class BackupController : ControllerBase
{
    private readonly SchoolDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public BackupController(SchoolDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpGet]
    public async Task<IActionResult> Download(CancellationToken cancellationToken)
    {
        var zipPath = Path.Combine(Path.GetTempPath(), $"school-backup-{Guid.NewGuid():N}.zip");
        try
        {
            // Read each table through EF Core. No SQL BACKUP command or database-wide lock is used.
            var backup = new
            {
                formatVersion = 1,
                createdAtUtc = DateTime.UtcNow,
                students = await _context.Students.AsNoTracking().ToListAsync(cancellationToken),
                teachers = await _context.Teachers.AsNoTracking().ToListAsync(cancellationToken),
                courses = await _context.Courses.AsNoTracking().ToListAsync(cancellationToken),
                classes = await _context.Classes.AsNoTracking().ToListAsync(cancellationToken),
                grades = await _context.Grades.AsNoTracking().ToListAsync(cancellationToken),
                attendances = await _context.Attendances.AsNoTracking().ToListAsync(cancellationToken),
                users = await _context.Users.AsNoTracking().ToListAsync(cancellationToken),
                marks = await _context.Marks.AsNoTracking().ToListAsync(cancellationToken),
                notifications = await _context.Notifications.AsNoTracking().ToListAsync(cancellationToken),
                homeworks = await _context.Homeworks.AsNoTracking().ToListAsync(cancellationToken)
            };

            await using (var file = new FileStream(zipPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, 81920, FileOptions.Asynchronous | FileOptions.SequentialScan))
            using (var archive = new ZipArchive(file, ZipArchiveMode.Create, leaveOpen: true))
            {
                var jsonEntry = archive.CreateEntry("database.json", CompressionLevel.Optimal);
                await using (var jsonStream = jsonEntry.Open())
                {
                    await JsonSerializer.SerializeAsync(jsonStream, backup, new JsonSerializerOptions { WriteIndented = true }, cancellationToken);
                }

                var uploadsPath = Path.Combine(_environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"), "uploads");
                archive.CreateEntry("uploads/");
                if (Directory.Exists(uploadsPath))
                {
                    foreach (var sourcePath in Directory.EnumerateFiles(uploadsPath, "*", SearchOption.AllDirectories))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var relativePath = Path.GetRelativePath(uploadsPath, sourcePath).Replace('\\', '/');
                        var entry = archive.CreateEntry($"uploads/{relativePath}", CompressionLevel.Optimal);
                        await using var source = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, FileOptions.Asynchronous | FileOptions.SequentialScan);
                        await using var destination = entry.Open();
                        await source.CopyToAsync(destination, cancellationToken);
                    }
                }
            }

            var download = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, FileOptions.Asynchronous | FileOptions.DeleteOnClose);
            return File(download, "application/zip", $"school-backup-{DateTime.UtcNow:yyyyMMdd-HHmmss}.zip");
        }
        finally
        {
            if (System.IO.File.Exists(zipPath))
            {
                try { System.IO.File.Delete(zipPath); }
                catch (IOException) { /* The download stream will remove it when it closes. */ }
            }
        }
    }
}
