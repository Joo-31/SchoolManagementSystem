using AutoMapper;
using SchoolManagementAPI.DTOs.Requests;
using SchoolManagementAPI.DTOs.Responses;
using SchoolManagementAPI.Models;
using SchoolManagementAPI.Repositories.Interfaces;
using SchoolManagementAPI.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace SchoolManagementAPI.Services
{
    public class HomeworkService : IHomeworkService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HomeworkService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void Create(CreateHomeworkDto dto, int teacherId)
        {
            var homework = _mapper.Map<Homework>(dto);
            homework.TeacherId = teacherId;
            homework.CreatedAt = DateTime.Now;

            _unitOfWork.Homeworks.Add(homework);
            _unitOfWork.SaveChanges();
        }

        public List<HomeworkDto> GetByTeacherId(int teacherId)
        {
            var list = _unitOfWork.Homeworks.GetByTeacherId(teacherId);
            return _mapper.Map<List<HomeworkDto>>(list);
        }

        public List<HomeworkDto> GetByStudentId(int studentId)
        {
            var list = _unitOfWork.Homeworks.GetByStudentId(studentId);
            return _mapper.Map<List<HomeworkDto>>(list);
        }

        public HomeworkDto? GetById(int id)
        {
            var hw = _unitOfWork.Homeworks.GetById(id);
            if (hw == null) return null;
            return _mapper.Map<HomeworkDto>(hw);
        }

        public void Delete(int id, int? teacherId = null)
        {
            var homework = _unitOfWork.Homeworks.GetById(id);
            if (homework == null) return;

            // لو جاي من مدرس، نتأكد إنه هو اللي عامل الواجب
            if (teacherId.HasValue && homework.TeacherId != teacherId.Value)
                throw new UnauthorizedAccessException("You can only delete your own homework.");

            _unitOfWork.Homeworks.Delete(homework);
            _unitOfWork.SaveChanges();
        }
    }
}