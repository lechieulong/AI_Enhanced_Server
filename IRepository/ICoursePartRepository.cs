using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.CourseFolder;

namespace IRepository
{
    public interface ICoursePartRepository
    {
        Task<IEnumerable<CoursePart>> GetAllAsync();
        Task<CoursePart> GetByIdAsync(Guid id);
        Task<CoursePart> AddAsync(CoursePart coursePart);
        Task<CoursePart> UpdateAsync(CoursePart coursePart);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<CoursePart>> GetByCourseIdAsync(Guid courseId);
        Task<IEnumerable<CoursePart>> GetByCourseSkillIdAsync(Guid courseSkillId); // New method
    }

}
