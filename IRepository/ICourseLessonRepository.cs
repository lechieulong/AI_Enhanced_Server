using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.CourseFolder;

namespace Repository
{
    public interface ICourseLessonRepository
    {
        Task<IEnumerable<CourseLesson>> GetAllAsync();
        Task<CourseLesson> GetByIdAsync(Guid id);
        Task<CourseLesson> AddAsync(CourseLesson courseLesson);
        Task<CourseLesson> UpdateAsync(CourseLesson courseLesson);
        Task<bool> DeleteAsync(Guid id);
    }
}
