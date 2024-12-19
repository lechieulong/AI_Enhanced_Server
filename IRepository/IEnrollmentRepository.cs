using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity;
using Entity.CourseFolder;
namespace Repositories
{
    public interface IEnrollmentRepository
    {
        Task<Enrollment> EnrollUserInCourse(Enrollment enrollment);
        Task<IEnumerable<Enrollment>> GetEnrollmentsByCourse(Guid courseId);
        Task<IEnumerable<Enrollment>> GetEnrollmentsByClass(Guid classId);
        Task<IEnumerable<Enrollment>> GetEnrollmentsByUser(string userId);
        Task<bool> CheckEnrollment(Guid courseId, string userId);
        Task<Enrollment> GetEnrollment(Guid courseId, string userId);
        Task<List<Guid>> GetClassIdsByEnrollment(Guid courseId, string userId);
        Task<IEnumerable<Enrollment>> GetByClassIdAsync(Guid classId);
        Task DeleteRangeAsync(IEnumerable<Enrollment> enrollments);
    }
}
