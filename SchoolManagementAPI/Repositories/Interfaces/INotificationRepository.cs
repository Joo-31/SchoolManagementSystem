using SchoolManagementAPI.Models;
using System.Collections.Generic;

namespace SchoolManagementAPI.Repositories.Interfaces
{
    public interface INotificationRepository : IRepository<Notification>
    {
        // بنجيب التنبيهات اللي تخص الـ Role بتاع المستخدم (أو All)
        IEnumerable<Notification> GetForRole(string role);
    }
}