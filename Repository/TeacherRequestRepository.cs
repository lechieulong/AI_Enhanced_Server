﻿using Common;
using Entity;
using Entity.Data;
using IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class TeacherRequestRepository : ITeacherRequestRepository
    {
        private readonly AppDbContext _context;
        public TeacherRequestRepository(AppDbContext db)
        {
            _context = db;
        }
        public async Task AddRequestAsync(TeacherRequest teacherRequest, UserEducation userEducation)
        {
            var existingRequest = await _context.TeacherRequests
                .FirstOrDefaultAsync(r => r.UserId == teacherRequest.UserId);

            if (existingRequest != null)
            {
                throw new InvalidOperationException("You have already submitted a request, please wait for approval.");
            }

            var existingEducation = await _context.UserEducations
                .FirstOrDefaultAsync(e => e.TeacherId == userEducation.TeacherId);

            if (existingEducation != null)
            {
                throw new InvalidOperationException("You have registered education information before, please wait for approval.");
            }

            // Sử dụng transaction để đảm bảo tất cả các bước đều thực hiện thành công hoặc rollback nếu có lỗi
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.UserEducations.Add(userEducation);

                if (userEducation.Specializations != null && userEducation.Specializations.Any())
                {
                    foreach (var specialization in userEducation.Specializations)
                    {
                        _context.Entry(specialization).State = EntityState.Unchanged;
                    }
                }

                _context.TeacherRequests.Add(teacherRequest);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("An error occurred while adding the teacher request and related information.", ex);
            }
        }

        public async Task<TeacherRequestDto> GetRequestByRequestId(Guid requestId)
        {
            var request = await _context.TeacherRequests
                .Include(tr => tr.User)
                .FirstOrDefaultAsync(tr => tr.Id == requestId);

            if (request == null)
            {
                return null;
            }

            var userEducation = await _context.UserEducations
                .FirstOrDefaultAsync(ue => ue.TeacherId == request.UserId);

            
            return new TeacherRequestDto
            {
                Id = request.Id,
                UserId = request.UserId,
                Description = request.Description,
                CreateAt = request.CreateAt,
                UpdateAt = request.UpdateAt,
                Status = (RequestStatusEnum)request.Status,
                User = new UserDto
                {
                    Name = request.User.Name,
                    Email = request.User.Email,
                    UserEducationDto = new UserEducationDto
                    {
                        AboutMe = userEducation.AboutMe,
                        Grade = userEducation.Grade,
                        DegreeURL = userEducation.DegreeURL,
                        Career = userEducation.Career,
                        YearExperience = userEducation.YearExperience,
                        SpecializationIds = userEducation.Specializations?.Select(s => s.Id).ToList() ?? new List<Guid>()
                    }
                }
            };
        }

        public async Task<TeacherRequest> GetRequestByUserId(string userId)
        {
            var teacherRequest = await _context.TeacherRequests
                .FirstOrDefaultAsync(r => r.UserId == userId);

            if (teacherRequest == null)
            {
                throw new KeyNotFoundException("No teacher request found for the specified user ID.");
            }

            return teacherRequest;
        }

        public async Task<(IEnumerable<TeacherRequestDto> requests, int totalCount)> GetTeacherRequestsAsync(int page, int pageSize, RequestStatusEnum status)
        {
            var query = _context.TeacherRequests
                .Where(tr => (RequestStatusEnum)tr.Status == status)
                .Include(tr => tr.User);

            var totalCount = await query.CountAsync();
            var requests = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(tr => new TeacherRequestDto
                {
                    Id = tr.Id,
                    UserId = tr.UserId,
                    Description = tr.Description,
                    CreateAt = tr.CreateAt,
                    UpdateAt = tr.UpdateAt,
                    Status = (RequestStatusEnum)tr.Status,
                    User = new UserDto
                    {
                        UserName = tr.User.UserName,
                        Name = tr.User.Name,
                        Email = tr.User.Email
                    }
                })
                .ToListAsync();

            return (requests, totalCount);
        }

    }
}
