﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.CourseFolder;

namespace Repository
{
    public interface ICoursePartRepository
    {
        Task<IEnumerable<CoursePart>> GetAllAsync();
        Task<CoursePart> GetByIdAsync(Guid id);
        Task<CoursePart> AddAsync(CoursePart coursePart);
        Task<CoursePart> UpdateAsync(CoursePart coursePart);
        Task<bool> DeleteAsync(Guid id);
    }
}
