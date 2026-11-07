using AutoMapper;
using SchoolManagementAPI.DTOs.Requests;
using SchoolManagementAPI.DTOs.Responses;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;
using SchoolManagementAPI.Services.Interfaces;
using System.Collections.Generic;

namespace SchoolManagementAPI.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void Create(CreateNotificationDto dto)
        {
            var notification = _mapper.Map<Notification>(dto);
            notification.CreatedAt = System.DateTime.Now;

            _unitOfWork.Notifications.Add(notification);
            _unitOfWork.SaveChanges();
        }

        public List<NotificationDto> GetForRole(string role)
        {
            var notifications = _unitOfWork.Notifications.GetForRole(role);
            return _mapper.Map<List<NotificationDto>>(notifications);
        }

        public void Delete(int id)
        {
            var notification = _unitOfWork.Notifications.GetById(id);
            if (notification != null)
            {
                _unitOfWork.Notifications.Delete(notification);
                _unitOfWork.SaveChanges();
            }
        }
    }
}