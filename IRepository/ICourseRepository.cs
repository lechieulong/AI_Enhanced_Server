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

        // Thêm phương thức để lấy tất cả khóa học với trạng thái IsEnabled
        Task<IEnumerable<Course>> GetAllEnabledCoursesAsync();

        // Thêm phương thức để lấy tất cả khóa học với trạng thái IsDisabled
        Task<IEnumerable<Course>> GetAllDisabledCoursesAsync();
    }
}
