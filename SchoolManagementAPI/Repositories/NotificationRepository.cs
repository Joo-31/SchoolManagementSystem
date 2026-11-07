using Microsoft.EntityFrameworkCore;
using SchoolManagementAPI.DataBase;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SchoolManagementAPI.Repositories
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(SchoolDbContext context) : base(context) { }

        public IEnumerable<Notification> GetForRole(string role)
        {
            return _dbSet
                .Where(n => n.TargetRole == "All" || n.TargetRole == role)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }
    }
}