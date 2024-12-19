using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Entity.CourseFolder;
using Entity.Data;
using IRepository;

namespace Repository
{
    public class CoursePartRepository : ICoursePartRepository
    {
        private readonly AppDbContext _context;

        public CoursePartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CoursePart>> GetAllAsync()
        {
            return await _context.Set<CoursePart>().ToListAsync();
        }

        public async Task<CoursePart> GetByIdAsync(Guid id)
        {
            return await _context.Set<CoursePart>().FindAsync(id);
        }

        public async Task<CoursePart> AddAsync(CoursePart coursePart)
        {
            await _context.Set<CoursePart>().AddAsync(coursePart);
            await _context.SaveChangesAsync();
            return coursePart;
        }
        public async Task<int> GetMaxOrderByCourseSkillIdAsync(Guid courseSkillId)
        {
            // Lấy giá trị lớn nhất của Order theo CourseSkillId
            return await _context.Set<CoursePart>()
                .Where(cp => cp.CourseSkillId == courseSkillId)
                .Select(cp => cp.Order)
                .DefaultIfEmpty(0) // Nếu không có bản ghi nào, trả về 0
                .MaxAsync();
        }


        public async Task<CoursePart> UpdateAsync(CoursePart coursePart)
        {
            _context.Set<CoursePart>().Update(coursePart);
            await _context.SaveChangesAsync();
            return coursePart;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var coursePart = await _context.Set<CoursePart>().FindAsync(id);
            if (coursePart == null)
                return false;

            _context.Set<CoursePart>().Remove(coursePart);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CoursePart>> GetByCourseIdAsync(Guid courseId)
        {
            return await _context.Set<CoursePart>()
                .Where(cp => cp.Skill.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<IEnumerable<CoursePart>> GetByCourseSkillIdAsync(Guid courseSkillId)
        {
            return await _context.Set<CoursePart>()
                .Where(cp => cp.CourseSkillId == courseSkillId)
                .OrderBy(cp => cp.Order)
                .ToListAsync();
        }

    }

}
