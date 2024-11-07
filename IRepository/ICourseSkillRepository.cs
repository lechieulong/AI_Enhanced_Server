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
        Task<IEnumerable<CourseSkill>> GetByCourseIdAsync(Guid courseId);
        Task<string> GetDescriptionByCourseLessonIdAsync(Guid courseLessonId);
        Task<string> GetDescriptionByCoursePartIdAsync(Guid coursePartId);
        Task<CourseSkill> GetBySkillIdAsync(Guid skillId);
    }
}
