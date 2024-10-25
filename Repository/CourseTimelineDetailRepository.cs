using Entity.Data;
using Entity;
using IRepository;
using Microsoft.EntityFrameworkCore;
using Model;

namespace Repository
{
    public class CourseTimelineDetailRepository : ICourseTimelineDetailRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<CourseTimelineDetail> _courseTimelineDetails; // Thay đổi về DbSet

        public CourseTimelineDetailRepository(AppDbContext context)
        {
            _context = context;
            _courseTimelineDetails = _context.CourseTimelineDetails; // Thay đổi về DbSet
        }

        public async Task<CourseTimelineDetailDto> GetByIdAsync(Guid id)
        {
            var courseTimelineDetail = await _courseTimelineDetails.FindAsync(id);
            return courseTimelineDetail != null ? MapToDto(courseTimelineDetail) : null; // Chuyển đổi sang DTO
        }

        public async Task<IEnumerable<CourseTimelineDetailDto>> GetAllAsync()
        {
            var courseTimelineDetails = await _courseTimelineDetails.ToListAsync();
            return courseTimelineDetails.Select(MapToDto); // Chuyển đổi danh sách sang DTO
        }

        public async Task CreateAsync(CourseTimelineDetailDto courseTimelineDetailDto)
        {
            var courseTimelineDetail = new CourseTimelineDetail
            {
                Id = Guid.NewGuid(), // Tạo ID mới
                CourseTimelineId = courseTimelineDetailDto.CourseTimelineId,
                Title = courseTimelineDetailDto.Title,
                VideoUrl = courseTimelineDetailDto.VideoUrl,
                Topic = courseTimelineDetailDto.Topic,
            };

            await _courseTimelineDetails.AddAsync(courseTimelineDetail);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CourseTimelineDetailDto courseTimelineDetailDto)
        {
            var courseTimelineDetail = await _courseTimelineDetails.FindAsync(courseTimelineDetailDto.Id);
            if (courseTimelineDetail != null)
            {
                courseTimelineDetail.CourseTimelineId = courseTimelineDetailDto.CourseTimelineId;
                courseTimelineDetail.Title = courseTimelineDetailDto.Title;
                courseTimelineDetail.VideoUrl = courseTimelineDetailDto.VideoUrl;
                courseTimelineDetail.Topic = courseTimelineDetailDto.Topic;

                _courseTimelineDetails.Update(courseTimelineDetail);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var courseTimelineDetail = await _courseTimelineDetails.FindAsync(id);
            if (courseTimelineDetail != null)
            {
                _courseTimelineDetails.Remove(courseTimelineDetail);
                await _context.SaveChangesAsync();
            }
        }

        // Phương thức lấy CourseTimelineDetail theo một CourseTimelineId duy nhất
        public async Task<IEnumerable<CourseTimelineDetailDto>> GetByCourseTimelineIdAsync(Guid courseTimelineId)
        {
            var courseTimelineDetails = await _courseTimelineDetails
                                 .Where(ct => ct.CourseTimelineId == courseTimelineId)
                                 .ToListAsync();

            return courseTimelineDetails.Select(MapToDto); // Chuyển đổi sang DTO
        }

        // Phương thức lấy CourseTimelineDetail theo danh sách CourseTimelineIds
        public async Task<IEnumerable<CourseTimelineDetailDto>> GetByCourseTimelineIdsAsync(IEnumerable<Guid> courseTimelineIds)
        {
            var courseTimelineDetails = await _courseTimelineDetails
                                 .Where(ct => courseTimelineIds.Contains(ct.CourseTimelineId))
                                 .ToListAsync();

            return courseTimelineDetails.Select(MapToDto); // Chuyển đổi sang DTO
        }

        // Hàm chuyển đổi từ CourseTimelineDetail sang CourseTimelineDetailDto
        private CourseTimelineDetailDto MapToDto(CourseTimelineDetail courseTimelineDetail)
        {
            return new CourseTimelineDetailDto
            {
                Id = courseTimelineDetail.Id,
                CourseTimelineId = courseTimelineDetail.CourseTimelineId,
                Title = courseTimelineDetail.Title,
                VideoUrl = courseTimelineDetail.VideoUrl,
                Topic = courseTimelineDetail.Topic,
            };
        }
    }
}
