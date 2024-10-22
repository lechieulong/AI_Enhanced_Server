using Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IRepository
{
    public interface ICourseRepository
    {
        Task<Course> GetByIdAsync(Guid id);
        Task<IEnumerable<Course>> GetAllAsync();
        Task CreateAsync(Course course);
        Task UpdateAsync(Course course);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Course>> GetAllCourseByUserIdAsync(string userId);
    }
}
