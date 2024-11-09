using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.CourseFolder;

namespace IRepository
{
    public interface ICourseLessonContentRepository
    {
        Task<IEnumerable<CourseLessonContent>> GetByCourseLessonIdAsync(Guid courseLessonId);
        Task<CourseLessonContent> GetByIdAsync(Guid id);
        Task<CourseLessonContent> AddAsync(CourseLessonContent content);
        Task UpdateAsync(CourseLessonContent content);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<CourseLessonContent>> GetAllAsync();
    }
}
