using Model;
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
        Task<CourseTimelineDetail> GetByIdAsync(int id);
        Task<IEnumerable<CourseTimelineDetail>> GetAllAsync();
        Task AddAsync(CourseTimelineDetail courseTimelineDetail);
        Task UpdateAsync(CourseTimelineDetail courseTimelineDetail);
        Task DeleteAsync(int id);

        Task<IEnumerable<CourseTimelineDetail>> GetByCourseTimelineIdAsync(int courseTimelineId); //Lấy tất cả CourseTimeline từ CourseId
    }
}
