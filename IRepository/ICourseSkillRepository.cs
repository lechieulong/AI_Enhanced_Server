using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.CourseFolder;

namespace Repository
{
    public interface ICourseSkillRepository
    {
        Task<IEnumerable<CourseSkill>> GetAllAsync();
        Task<CourseSkill> GetByIdAsync(Guid id);
        Task<CourseSkill> AddAsync(CourseSkill courseSkill);
        Task<CourseSkill> UpdateAsync(CourseSkill courseSkill);
        Task<bool> DeleteAsync(Guid id);
    }
}
