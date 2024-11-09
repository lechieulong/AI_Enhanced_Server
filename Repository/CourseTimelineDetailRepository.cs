using Entity.Data;
using Entity;
using Microsoft.EntityFrameworkCore;
using Model;
using Entity.CourseFolder;
public class CourseTimelineDetailRepository : ICourseTimelineDetailRepository
{
    private readonly AppDbContext _context;
    private readonly DbSet<CourseTimelineDetail> _courseTimelineDetails;

    public CourseTimelineDetailRepository(AppDbContext context)
    {
        _context = context;
        _courseTimelineDetails = _context.CourseTimelineDetails;
    }

    // Lấy CourseTimelineDetail theo ID
    public async Task<CourseTimelineDetailDto> GetByIdAsync(Guid id)
    {
        var courseTimelineDetail = await _courseTimelineDetails.FindAsync(id);
        return courseTimelineDetail != null ? MapToDto(courseTimelineDetail) : null;
    }

    // Lấy tất cả CourseTimelineDetail
    public async Task<IEnumerable<CourseTimelineDetailDto>> GetAllAsync()
    {
        var courseTimelineDetails = await _courseTimelineDetails.ToListAsync();
        return courseTimelineDetails.Select(MapToDto);
    }

    public async Task CreateAsync(CourseTimelineDetail courseTimelineDetail)
    {
        if (courseTimelineDetail == null)
        {
            throw new ArgumentNullException(nameof(courseTimelineDetail), "CourseTimeline cannot be null.");
        }
        await _courseTimelineDetails.AddAsync(courseTimelineDetail);
        await _context.SaveChangesAsync();
    }

    // Cập nhật CourseTimelineDetail
    public async Task<bool> UpdateAsync(CourseTimelineDetailDto courseTimelineDetailDto)
    {
        // Tìm bằng CourseTimelineId hoặc tạo mới nếu không tìm thấy
        var courseTimelineDetail = await _courseTimelineDetails
            .FirstOrDefaultAsync(ct => ct.CourseTimelineId == courseTimelineDetailDto.CourseTimelineId);

        if (courseTimelineDetail != null)
        {
            // Cập nhật các trường
            courseTimelineDetail.Title = courseTimelineDetailDto.Title;
            courseTimelineDetail.VideoUrl = courseTimelineDetailDto.VideoUrl;
            courseTimelineDetail.Topic = courseTimelineDetailDto.Topic;
            courseTimelineDetail.IsEnabled = courseTimelineDetailDto.IsEnabled;

            _courseTimelineDetails.Update(courseTimelineDetail);
            await _context.SaveChangesAsync();
            return true; // Trả về true nếu cập nhật thành công
        }
        return false; // Trả về false nếu không tìm thấy
    }

    // Xóa CourseTimelineDetail theo ID
    public async Task<bool> DeleteAsync(Guid id)
    {
        var courseTimelineDetail = await _courseTimelineDetails.FindAsync(id);
        if (courseTimelineDetail != null)
        {
            _courseTimelineDetails.Remove(courseTimelineDetail);
            await _context.SaveChangesAsync();
            return true; // Trả về true nếu xóa thành công
        }
        return false; // Trả về false nếu không tìm thấy
    }

    // Lấy tất cả CourseTimelineDetail theo CourseTimelineId
    public async Task<IEnumerable<CourseTimelineDetailDto>> GetByCourseTimelineIdAsync(Guid courseTimelineId)
    {
        var courseTimelineDetails = await _courseTimelineDetails
                             .Where(ct => ct.CourseTimelineId == courseTimelineId)
                             .ToListAsync();

        return courseTimelineDetails.Select(MapToDto);
    }

    // Lấy CourseTimelineDetail theo danh sách CourseTimelineIds
    public async Task<IEnumerable<CourseTimelineDetailDto>> GetByCourseTimelineIdsAsync(IEnumerable<Guid> courseTimelineIds)
    {
        var courseTimelineDetails = await _courseTimelineDetails
                             .Where(ct => courseTimelineIds.Contains(ct.CourseTimelineId))
                             .ToListAsync();

        return courseTimelineDetails.Select(MapToDto);
    }

    // Chuyển đổi từ CourseTimelineDetail sang CourseTimelineDetailDto
    private CourseTimelineDetailDto MapToDto(CourseTimelineDetail entity)
    {
        return new CourseTimelineDetailDto
        {
            Id = entity.Id,
            CourseTimelineId = entity.CourseTimelineId,
            Title = entity.Title,
            VideoUrl = entity.VideoUrl,
            Topic = entity.Topic,
            IsEnabled = entity.IsEnabled
        };
    }

}
