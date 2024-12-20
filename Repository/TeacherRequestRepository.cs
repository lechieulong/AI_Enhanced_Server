using Azure.Core;
using Common;
using Entity;
using Entity.Data;
using Entity.Test;
using IRepository;
using IRepository.Live;
using IService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Utility;
using Repository.Live;
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
        private readonly IEmailSenderService _emailSenderService;
        private readonly IAuthRepository _authRepository;
        private readonly ILiveStreamRepository _liveStreamRepository;
        public TeacherRequestRepository(AppDbContext db, IEmailSenderService emailSenderService, IAuthRepository authRepository, ILiveStreamRepository liveStreamRepository)
        {
            _emailSenderService = emailSenderService;
            _authRepository = authRepository;
            _liveStreamRepository = liveStreamRepository;
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

            // Sử dụng ExecutionStrategy để bao bọc giao dịch
            var executionStrategy = _context.Database.CreateExecutionStrategy();
            await executionStrategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

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
            });
        }

        public async Task<TestExam> GetRandomAdminTestAsync()
        {
            var randomTest = await _context.Set<TestExam>()
                .Where(te => te.TestCreateBy == 1)
                .OrderBy(te => Guid.NewGuid())
                .FirstOrDefaultAsync();

            return randomTest;
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

        public async Task<TeacherRequestDto> ProcessRequest(Guid requestId, ProcessTeacherRequestDto processTeacherRequestDto)
        {
            var request = await _context.TeacherRequests.FindAsync(requestId);
            if (request == null)
            {
                throw new KeyNotFoundException("Request not found");
            }

            request.Description = processTeacherRequestDto.Comment;
            request.Status = (int)processTeacherRequestDto.Status; // Using the enum to set the status
            request.UpdateAt = DateTime.Now;

            await _context.SaveChangesAsync();

            // Get user information for sending the email
            var user = await _context.Users.FindAsync(request.UserId);
            if (user != null)
            {
                try
                {
                    if (request.Status == (int)RequestStatusEnum.Approve) // Status is 1
                    {
                        var assignRoleSuccessful = await _authRepository.AssignRole(user.Email, SD.Teacher);
                        if (!assignRoleSuccessful)
                        {
                            // Handle role assignment failure (logging, exception, etc.)
                            throw new InvalidOperationException($"Failed to assign role to {user.Email}");
                        }

                        await _liveStreamRepository.AddLiveStreamAsync(Guid.Parse(request.UserId));
                        await _emailSenderService.SendApproveTeacherRequestEmail(user.Email, user.Name, request.Description);
                    }
                    else if (request.Status == (int)RequestStatusEnum.Reject) // Status is 2
                    {
                        await _emailSenderService.SendRejectTeacherRequestEmail(user.Email, user.Name, request.Description);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while processing the request.", ex);
                }
            }
            // Return updated request DTO
            var updatedRequestDto = new TeacherRequestDto
            {
                Id = request.Id,
                UserId = request.UserId,
                Description = request.Description,
                Status = (RequestStatusEnum)request.Status,
                CreateAt = request.CreateAt,
                UpdateAt = request.UpdateAt
            };

            return updatedRequestDto;
        }

        public async Task UpdateRequestAsync(TeacherRequest teacherRequest, UserEducation userEducation)
        {
            // Get existing request
            var existingRequest = await _context.TeacherRequests
                .Include(tr => tr.User)
                .FirstOrDefaultAsync(r => r.Id == teacherRequest.Id);

            if (existingRequest == null)
            {
                throw new KeyNotFoundException("Teacher request not found.");
            }

            // Update request details
            existingRequest.Description = teacherRequest.Description;
            existingRequest.Status = (int)RequestStatusEnum.Pending;
            existingRequest.UpdateAt = DateTime.Now;

            // Get or add user education
            var existingEducation = await _context.UserEducations
                .Include(e => e.Specializations) // Load specializations for updates
                .FirstOrDefaultAsync(e => e.TeacherId == userEducation.TeacherId);

            if (existingEducation != null)
            {
                // Update existing user education details
                existingEducation.AboutMe = userEducation.AboutMe;
                existingEducation.Grade = userEducation.Grade;
                existingEducation.DegreeURL = userEducation.DegreeURL;
                existingEducation.Career = userEducation.Career;
                existingEducation.YearExperience = userEducation.YearExperience;

                // Update specializations
                existingEducation.Specializations.Clear();

                foreach (var specialization in userEducation.Specializations)
                {
                    var specializationObject = GetSpecialization(specialization.Id);

                    existingEducation.Specializations.Add(specializationObject);
                }
            }

            // Save all changes to the database
            await _context.SaveChangesAsync();
        }

        private Specialization GetSpecialization(Guid specializationId)
        {
            var specialization = _context.Specializations.Find(specializationId);
            return specialization;
        }

    }
}
