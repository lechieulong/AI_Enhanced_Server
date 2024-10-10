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
    public class TeacherScheduleRepository : ITeacherScheduleRepository
    {
        private readonly AppDbContext _db;

        public TeacherScheduleRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<TeacherAvailableSchedule> CreateAsync(TeacherAvailableSchedule newSchedule)
        {
            if (newSchedule.StartTime < DateTime.Now)
            {
                throw new ArgumentException("Start time cannot be in the past.");
            }
            if (newSchedule.Minutes <= 0)
            {
                throw new ArgumentException("Duration should be greater than zero.");
            }
            if (newSchedule.Price <= 0)
            {
                throw new ArgumentException("Price must be a positive value.");
            }
            // Create the schedule
            await _db.TeacherAvailableSchedules.AddAsync(newSchedule);
            await _db.SaveChangesAsync();
            //// Automatically create an Event based on the new TeacherAvailableSchedule
            //var newEvent = new Event
            //{
            //    Title = $"Available Coaching Session with {newSchedule.TeacherId}",
            //    Description = $"Coaching session for {newSchedule.Minutes} minutes",
            //    Start = newSchedule.StartTime,
            //    End = newSchedule.StartTime.AddMinutes(newSchedule.Minutes),
            //    Link = newSchedule.Link,
            //    UserId = newSchedule.TeacherId, // Assuming the Teacher is the "User" who creates this event
            //    User = newSchedule.Teacher // Optionally set the Teacher entity if needed
            //};
            //await _db.Events.AddAsync(newEvent);
            //await _db.SaveChangesAsync();
            return newSchedule;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var schedule = await _db.TeacherAvailableSchedules.FirstOrDefaultAsync(p => p.Id == id);
            if (schedule != null)
            {
                _db.TeacherAvailableSchedules.Remove(schedule);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<TeacherAvailableSchedule>> GetAllAsync()
        {
            return await _db.TeacherAvailableSchedules.ToListAsync();
        }

        public async Task<TeacherAvailableSchedule> GetByIdAsync(Guid id)
        {
            return await _db.TeacherAvailableSchedules.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<TeacherAvailableSchedule>> GetByTeacherNameAsync(string userName)
        {
            return await _db.TeacherAvailableSchedules
                            .Where(p => p.Teacher.UserName == userName)
                            .ToListAsync();
        }

        public async Task<TeacherAvailableSchedule> UpdateAsync(TeacherAvailableSchedule updatedSchedule)
        {
            _db.TeacherAvailableSchedules.Update(updatedSchedule);
            await _db.SaveChangesAsync();
            return updatedSchedule;
        }

        public async Task<IEnumerable<TeacherAvailableSchedule>> GetConflictingSchedulesAsync(string teacherId, DateTime startTime, DateTime endTime)
        {
            return await _db.TeacherAvailableSchedules
                .Where(schedule => schedule.TeacherId == teacherId &&
                                  ((schedule.StartTime.AddMinutes(schedule.Minutes) > startTime) && (schedule.StartTime < endTime)))
                .ToListAsync();
        }

    }
}
