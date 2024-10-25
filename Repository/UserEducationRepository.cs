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
    public class UserEducationRepository : IUserEducationRepository
    {
        private readonly AppDbContext _context;
        public UserEducationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserEducation> CreateAsync(UserEducation userEducation)
        {
            var existingEducation = await _context.UserEducations
                .FirstOrDefaultAsync(e => e.TeacherId == userEducation.TeacherId);

            if (existingEducation != null)
            {
                throw new InvalidOperationException("You have registered before, please wait for approve.");
            }

            _context.UserEducations.Add(userEducation);

            if (userEducation.Specializations != null && userEducation.Specializations.Any())
            {
                foreach (var specialization in userEducation.Specializations)
                {
                    _context.Entry(specialization).State = EntityState.Unchanged;
                }
            }

            await _context.SaveChangesAsync();
            return userEducation;
        }

        public async Task DeleteAsync(string teacherId)
        {
            var entity = await GetByIdAsync(teacherId);
            if (entity != null)
            {
                _context.Set<UserEducation>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<UserEducation>> GetAllAsync()
        {
            return await _context.Set<UserEducation>().ToListAsync();
        }

        public async Task<UserEducation> GetByIdAsync(string teacherId)
        {
            return await _context.Set<UserEducation>().FindAsync(teacherId);
        }

        public async Task<UserEducation> UpdateAsync(UserEducation userEducation)
        {
            _context.Entry(userEducation).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return userEducation;
        }
    }
}
