using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity;
using Entity.CourseFolder;
using Model;

namespace IRepository
{
    public interface IClassFileRepository
    {
        Task<IEnumerable<ClassFileDto>> GetAllByClassIdAsync(Guid classId);
        Task<ClassFileDto> GetByIdAsync(Guid id);
        Task CreateAsync(ClassFile classFile);
        Task<bool> DeleteAsync(Guid id);
        Task<ClassFileDto> UpdateAsync(Guid id, ClassFileDto updatedClassFile);
    }
}
