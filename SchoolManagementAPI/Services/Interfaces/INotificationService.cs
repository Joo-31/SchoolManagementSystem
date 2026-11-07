using SchoolManagementAPI.DTOs.Requests;
using SchoolManagementAPI.DTOs.Responses;
using System.Collections.Generic;

namespace SchoolManagementAPI.Services.Interfaces
{
    public interface INotificationService
    {
        void Create(CreateNotificationDto dto);
        List<NotificationDto> GetForRole(string role);
        void Delete(int id);
    }
}