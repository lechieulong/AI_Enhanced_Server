using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity;

namespace IRepository
{
    public interface ICourseRepository
    {
        Task<List<Course>> GetAllAsync();
        Task<Course> GetByIdAsync(Guid id);
        Task CreateAsync(Course course);
        Task UpdateAsync(Course course);
        Task DeleteAsync(Guid id);
        Task<List<Course>> GetAllEnabledCoursesAsync();
        Task<List<Course>> GetAllDisabledCoursesAsync();
        Task<List<Course>> GetAllCourseByUserIdAsync(string userId);
        Task UpdateCourseEnabledStatusAsync(Guid courseId, bool isEnabled);
    }
}
