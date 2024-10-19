using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IRepository
{
    public interface ICourseTimelineDetailRepository
    {
        Task<CourseTimelineDetail> GetByIdAsync(Guid id);
        Task<IEnumerable<CourseTimelineDetail>> GetAllAsync();
        Task CreateAsync(CourseTimelineDetail courseTimelineDetail);
        Task UpdateAsync(CourseTimelineDetail courseTimelineDetail);
        Task DeleteAsync(Guid id);

        Task<IEnumerable<CourseTimelineDetail>> GetByCourseTimelineIdAsync(Guid courseTimelineId); //Lấy tất cả CourseTimeline từ CourseId
    }
}
