using Entity;
using Model;

namespace IRepository
{
    public interface ICourseTimelineDetailRepository
    {
        Task<CourseTimelineDetailDto> GetByIdAsync(Guid id);
        Task<IEnumerable<CourseTimelineDetailDto>> GetAllAsync();
        Task CreateAsync(CourseTimelineDetailDto courseTimelineDetail);
        Task UpdateAsync(CourseTimelineDetailDto courseTimelineDetail);
        Task DeleteAsync(Guid id);

        // Phương thức lấy tất cả CourseTimelineDetail theo một CourseTimelineId duy nhất
        Task<IEnumerable<CourseTimelineDetailDto>> GetByCourseTimelineIdAsync(Guid courseTimelineId);

        // Thêm phương thức để lấy chi tiết theo danh sách CourseTimelineIds
        Task<IEnumerable<CourseTimelineDetailDto>> GetByCourseTimelineIdsAsync(IEnumerable<Guid> courseTimelineIds);
    }
}
