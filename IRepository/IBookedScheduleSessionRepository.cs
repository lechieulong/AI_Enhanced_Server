using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository
{
    public interface IBookedScheduleSessionRepository
    {
        Task<BookedTeacherSession> CreateScheduleSessionAsync(BookedTeacherSession bookedTeacherSession);
        Task<IEnumerable<BookedTeacherSession>> GetSessionsByUserIdAsync(string userId);
    }
}
