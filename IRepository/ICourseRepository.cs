using Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IRepository
{
    public interface ICourseRepository
    {
        Task<Course> GetByIdAsync(Guid id); // Thay đổi từ int sang Guid
        Task<IEnumerable<Course>> GetAllAsync();
        Task AddAsync(Course course);
        Task UpdateAsync(Course course);
        Task DeleteAsync(Guid id); // Thay đổi từ int sang Guid
        Task<IEnumerable<Course>> GetAllByUserIdAsync(string userId);
    }
}
