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

        public async Task<IEnumerable<TeacherAvailableSchedule>> GetByTeacherIdAsync(string userName)
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
    }
}
