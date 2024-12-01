using Common;
using Entity;
using Entity.Data;
using IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class BookedScheduleSessionRepository : IBookedScheduleSessionRepository
    {
        private readonly AppDbContext _db;

        public BookedScheduleSessionRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task<BookedTeacherSession> CreateScheduleSessionAsync(BookedTeacherSession bookedTeacherSession)
        {
            if (bookedTeacherSession == null || bookedTeacherSession.ScheduleId == Guid.Empty || string.IsNullOrEmpty(bookedTeacherSession.LearnerId))
            {
                throw new ArgumentException("Invalid booking session data.");
            }

            try
            {
                bookedTeacherSession.Id = Guid.NewGuid();
                bookedTeacherSession.BookedDate = DateTime.Now;
                await _db.BookedTeacherSessions.AddAsync(bookedTeacherSession);

                // Update schedule status to "booked"
                var schedule = await _db.TeacherAvailableSchedules.FindAsync(bookedTeacherSession.ScheduleId);
                if (schedule != null)
                {
                    schedule.Status = (int)ScheduleStatus.Booked;
                    _db.TeacherAvailableSchedules.Update(schedule);
                }

                await _db.SaveChangesAsync();

                return bookedTeacherSession;
            }
            catch (Exception ex)
            {
                // Log the error
                throw new Exception("An error occurred while booking the session.", ex);
            }
        }

        public async Task<IEnumerable<BookedTeacherSession>> GetSessionsByUserIdAsync(string userId)
        {
            return await _db.BookedTeacherSessions
                .Include(b => b.TeacherAvailableSchedule)
                .Where(b => b.LearnerId == userId)
                .ToListAsync();
        }

    }
}
