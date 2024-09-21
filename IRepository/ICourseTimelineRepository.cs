using Entity;

public interface ICourseTimelineRepository
{
    Task<CourseTimeline> GetByIdAsync(int id);
    Task<IEnumerable<CourseTimeline>> GetAllAsync();
    Task AddAsync(CourseTimeline courseTimeline);
    Task UpdateAsync(CourseTimeline courseTimeline);
    Task DeleteAsync(int id);
    Task<bool> CheckExistCourseIdAsync(int courseId);
    Task<IEnumerable<CourseTimeline>> GetByCourseIdAsync(int courseId);
}