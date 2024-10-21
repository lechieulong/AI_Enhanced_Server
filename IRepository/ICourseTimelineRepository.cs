using Entity;

public interface ICourseTimelineRepository
{
    Task<CourseTimeline> GetByIdAsync(Guid id);
    Task<IEnumerable<CourseTimeline>> GetAllAsync();
    Task AddAsync(CourseTimeline courseTimeline);
    Task UpdateAsync(CourseTimeline courseTimeline);
    Task DeleteAsync(Guid id);
    Task<bool> CheckExistCourseIdAsync(Guid courseId);
    Task<IEnumerable<CourseTimeline>> GetByCourseIdAsync(Guid courseId);
}