using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity;
using Entity.CourseFolder;
namespace IRepository
{
    public interface ICourseRepository
    {
        Task<int> CountAsync();
        Task<List<Course>> GetAllAsync(int pageNumber, int pageSize);

        Task<Course> GetByIdAsync(Guid id);
        Task CreateAsync(Course course);
        Task UpdateAsync(Course course);
        Task DeleteAsync(Guid id);
        Task<List<Course>> GetAllEnabledCoursesAsync();
        Task<List<Course>> GetAllDisabledCoursesAsync();
        Task<List<Course>> GetAllCourseByUserIdAsync(string userId);
        Task<List<Course>> GetCreatedCourses(string userId);
        Task UpdateCourseEnabledStatusAsync(Guid courseId, bool isEnabled);
        Task<Guid?> GetCourseIdByLessonContentIdAsync(Guid courseLessonContentId);
        Task<bool> CheckLecturerOfCourse(string userId, Guid courseId);
        Task UpdateCourseRatingAsync(Guid courseId);
        Task AddRatingAsync(CourseRating rating);
        Task<bool> HasClassesAsync(Guid courseId);

    }
}
