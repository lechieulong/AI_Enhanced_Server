using Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICourseTimelineRepository
{
    Task<CourseTimeline> GetByIdAsync(Guid id);
    Task<IEnumerable<CourseTimeline>> GetAllAsync();
    Task CreateAsync(CourseTimeline courseTimeline);
    Task UpdateAsync(CourseTimeline courseTimeline);
    Task DeleteAsync(Guid id);
    Task<bool> CheckExistCourseIdAsync(Guid courseId);
    Task<IEnumerable<CourseTimeline>> GetByCourseIdAsync(Guid courseId);
    Task<List<CourseTimeline>> GetCourseTimelinesByCourseIdAsync(Guid courseId);
}