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

        public async Task<IEnumerable<TeacherAvailableSchedule>> GetByTeacherName7DaysAsync(string userName)
        {
            var today = DateTime.Now;
            var sevenDaysFromNow = today.AddDays(6);

            return await _db.TeacherAvailableSchedules
                            .Where(p => p.Teacher.UserName == userName &&
                                        p.StartTime >= today &&
                                        p.StartTime <= sevenDaysFromNow &&
                                        (p.Status == 0 || p.Status == 1))
                            .OrderBy(p => p.StartTime)
                            .ToListAsync();
        }

        public async Task<TeacherAvailableSchedule> UpdateAsync(TeacherAvailableSchedule updatedSchedule)
        {
            if (updatedSchedule.StartTime < DateTime.Now)
            {
                throw new ArgumentException("Start time cannot be in the past.");
            }
            if (updatedSchedule.Minutes <= 0)
            {
                throw new ArgumentException("Duration should be greater than zero.");
            }
            if (updatedSchedule.Price <= 0)
            {
                throw new ArgumentException("Price must be a positive value.");
            }
            var existingSchedule = await _db.TeacherAvailableSchedules
                                            .FirstOrDefaultAsync(s => s.Id == updatedSchedule.Id);
            if (existingSchedule == null)
            {
                throw new KeyNotFoundException("The schedule to update was not found.");
            }
            existingSchedule.Content = updatedSchedule.Content;
            existingSchedule.StartTime = updatedSchedule.StartTime;
            existingSchedule.Minutes = updatedSchedule.Minutes;
            existingSchedule.Price = updatedSchedule.Price;
            existingSchedule.Link = updatedSchedule.Link;
            // Save changes
            await _db.SaveChangesAsync();

            return existingSchedule; // Return the updated schedule
        }

        public async Task<IEnumerable<TeacherAvailableSchedule>> GetConflictingSchedulesAsync(string teacherId, DateTime startTime, DateTime endTime)
        {
            return await _db.TeacherAvailableSchedules
                .Where(schedule => schedule.TeacherId == teacherId &&
                                  ((schedule.StartTime.AddMinutes(schedule.Minutes) > startTime) && (schedule.StartTime < endTime)))
                .ToListAsync();
        }

        public async Task<IEnumerable<TeacherAvailableSchedule>> GetConflictingSchedulesAsync(string teacherId, DateTime startTime, DateTime endTime, Guid currentScheduleId)
        {
            return await _db.TeacherAvailableSchedules
                .Where(schedule => schedule.TeacherId == teacherId &&
                                  schedule.Id != currentScheduleId && // Exclude the current schedule being updated
                                  ((schedule.StartTime.AddMinutes(schedule.Minutes) > startTime) && (schedule.StartTime < endTime)))
                .ToListAsync();
        }


        public async Task<IEnumerable<TeacherAvailableSchedule>> GetAllPendingAsync()
        {
            return await _db.TeacherAvailableSchedules
                .Where(schedule => schedule.Status == (int)ScheduleStatus.Pending)
                .ToListAsync();
        }
    }
}