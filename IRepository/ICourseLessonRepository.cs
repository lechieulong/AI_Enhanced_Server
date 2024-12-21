using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.CourseFolder;

namespace IRepository
{
    public interface ICourseLessonRepository
    {
        Task<IEnumerable<CourseLesson>> GetAllAsync();
        Task<CourseLesson> GetByIdAsync(Guid id);
        Task<CourseLesson> AddAsync(CourseLesson courseLesson);
        Task<CourseLesson> UpdateAsync(CourseLesson courseLesson);
        Task<IEnumerable<CourseLesson>> GetByCoursePartIdAsync(Guid coursePartId);
        Task<bool> DeleteAsync(Guid id);
        Task<int> GetMaxOrderByCoursePartIdAsync(Guid coursePartId);
    }
}
