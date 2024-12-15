using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity;
using Entity.Data;
using Entity.CourseFolder;
using Microsoft.EntityFrameworkCore;
using Entity.CourseFolder;
namespace Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly AppDbContext _context;

        public EnrollmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Enrollment> EnrollUserInCourse(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByCourse(Guid courseId)
        {
            return await _context.Enrollments
                .Include(uc => uc.User)
                .Include(uc => uc.Course)
                .Where(uc => uc.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByClass(Guid classId)
        {
            return await _context.Enrollments
                .Include(uc => uc.User)
                .Include(uc => uc.Class)
                .Where(uc => uc.ClassId == classId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByUser(string userId)
        {
            return await _context.Enrollments
                .Include(uc => uc.Course)
                .Where(uc => uc.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> CheckEnrollment(Guid courseId, string userId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.CourseId == courseId && e.UserId == userId);
        }
        public async Task<Enrollment> GetEnrollment(Guid courseId, string userId)
        {
            return await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == userId);
        }
        public async Task<List<Guid>> GetClassIdsByEnrollment(Guid courseId, string userId)
        {
            return await _context.Enrollments
                .Where(e => e.CourseId == courseId && e.UserId == userId)
                .Select(e => e.ClassId)
                .ToListAsync();

        }
    }

}
