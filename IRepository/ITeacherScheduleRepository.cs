using Entity;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository
{
    public interface ITeacherScheduleRepository
    {
        Task<TeacherAvailableSchedule> GetByIdAsync(Guid id);
        Task<IEnumerable<TeacherAvailableSchedule>> GetAllAsync();
        Task<IEnumerable<TeacherAvailableSchedule>> GetByTeacherNameAsync(string userName);
        Task<TeacherAvailableSchedule> CreateAsync(TeacherAvailableSchedule newSchedule);
        Task<TeacherAvailableSchedule> UpdateAsync(TeacherAvailableSchedule updatedSchedule);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<TeacherAvailableSchedule>> GetConflictingSchedulesAsync(string teacherId, DateTime startTime, DateTime endTime);
    }

}
