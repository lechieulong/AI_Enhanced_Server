using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Entity.CourseFolder;
using Entity.Data;
using IRepository;

namespace Repository
{
    public class CourseSkillRepository : ICourseSkillRepository
    {
        private readonly AppDbContext _context;

        public CourseSkillRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CourseSkill>> GetAllAsync()
        {
            return await _context.Set<CourseSkill>().ToListAsync();
        }

        public async Task<CourseSkill> GetByIdAsync(Guid id)
        {
            return await _context.Set<CourseSkill>().FindAsync(id);
        }

        public async Task<CourseSkill> AddAsync(CourseSkill courseSkill)
        {
            await _context.Set<CourseSkill>().AddAsync(courseSkill);
            await _context.SaveChangesAsync();
            return courseSkill;
        }

        public async Task<CourseSkill> UpdateAsync(CourseSkill courseSkill)
        {
            _context.Set<CourseSkill>().Update(courseSkill);
            await _context.SaveChangesAsync();
            return courseSkill;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var courseSkill = await _context.Set<CourseSkill>().FindAsync(id);
            if (courseSkill == null)
                return false;

            _context.Set<CourseSkill>().Remove(courseSkill);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<CourseSkill>> GetByCourseIdAsync(Guid courseId)
        {
            return await _context.CourseSkills
                                 .Where(skill => skill.CourseId == courseId)
                                 .ToListAsync();
        }
        public async Task<string> GetDescriptionByCourseLessonIdAsync(Guid courseLessonId)
        {
            var description = await _context.CourseLessons
                .Where(cl => cl.Id == courseLessonId)
                .Select(cl => cl.CoursePart.Skill.Description)
                .FirstOrDefaultAsync();

            return description;
        }
        public async Task<string> GetDescriptionByCoursePartIdAsync(Guid coursePartId)
        {
            var description = await _context.CourseParts
                .Where(cp => cp.Id == coursePartId)
                .Select(cp => cp.Skill.Description)
                .FirstOrDefaultAsync();

            return description;
        }
        public async Task<CourseSkill> GetBySkillIdAsync(Guid skillId)
        {
            return await _context.CourseSkills
                                 .Where(cs => cs.Id == skillId)
                                 .Select(cs => new CourseSkill { Id = cs.Id, Description = cs.Description })
                                 .FirstOrDefaultAsync();
        }
    }
}
