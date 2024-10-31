using Entity;
using Model;

public interface ICourseTimelineDetailRepository
{
    Task<CourseTimelineDetailDto> GetByIdAsync(Guid id);
    Task<IEnumerable<CourseTimelineDetailDto>> GetAllAsync();

    Task CreateAsync(CourseTimelineDetail courseTimelineDetail);
    Task<bool> UpdateAsync(CourseTimelineDetailDto courseTimelineDetailDto); // Sử dụng CourseTimelineDetailDto
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<CourseTimelineDetailDto>> GetByCourseTimelineIdAsync(Guid courseTimelineId);
    Task<IEnumerable<CourseTimelineDetailDto>> GetByCourseTimelineIdsAsync(IEnumerable<Guid> courseTimelineIds);
}
