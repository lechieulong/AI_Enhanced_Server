using Entity;
using Entity.Data;
using IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

    }
}
