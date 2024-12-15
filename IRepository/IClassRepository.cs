﻿using Entity;
using Entity.CourseFolder;
using Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IRepository
{
    public interface IClassRepository
    {
        Task<ClassDto> CreateAsync(Class newClass);
        Task<ClassDto> GetByIdAsync(Guid id);
        Task<IEnumerable<ClassDto>> GetByCourseIdAsync(Guid courseId);
        Task<IEnumerable<ClassDto>> GetAllAsync();
        Task<ClassDto> UpdateAsync(Guid classId, ClassDto updatedClass);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<ClassDto>> GetByTeacherIdAsync(string teacherId);
        Task UpdateClassEnabledStatusAsync(Guid classId, bool isEnabled);
        Task<Enrollment> GetEnrollmentAsync(Guid courseId, string userId);
        Task<Class> GetEntityByIdAsync(Guid classId);
        Task<List<object>> GetUnenrolledClassesAsync(Guid courseId, string userId);
    }
}
