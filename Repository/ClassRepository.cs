using IRepository;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Entity.Data;
using Entity;

namespace Repository
{
    public class ClassRepository : IClassRepository
    {
        private readonly AppDbContext _dbContext;

        public ClassRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ClassDto>> GetAllAsync()
        {
            return await _dbContext.Classes
                .Select(c => new ClassDto
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    ClassDescription = c.ClassDescription,
                    Count = c.Count, // Ensure Count is handled as int
                    CourseId = c.CourseId
                }).ToListAsync();
        }

        public async Task<ClassDto> GetByIdAsync(Guid id)
        {
            var classEntity = await _dbContext.Classes.FindAsync(id);
            if (classEntity == null)
            {
                return null;
            }

            return new ClassDto
            {
                Id = classEntity.Id,
                ClassName = classEntity.ClassName,
                ClassDescription = classEntity.ClassDescription,
                Count = classEntity.Count,
                CourseId = classEntity.CourseId
            };
        }

        public async Task<ClassDto> CreateAsync(ClassDto newClassDto)
        {
            if (newClassDto == null)
            {
                throw new ArgumentNullException(nameof(newClassDto), "Class DTO cannot be null.");
            }

            var classEntity = new Class
            {
                ClassName = newClassDto.ClassName,
                ClassDescription = newClassDto.ClassDescription,
                Count = newClassDto.Count, // This should be managed accordingly
                CourseId = newClassDto.CourseId,
                StartDate = newClassDto.StartDate,
                ImageUrl = newClassDto.ImageUrl
            };

            try
            {
                await _dbContext.Classes.AddAsync(classEntity);
                await _dbContext.SaveChangesAsync();

                newClassDto.Id = classEntity.Id;

                return newClassDto;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while creating the class.", ex);
            }
        }

        public async Task<ClassDto> UpdateAsync(ClassDto updatedClassDto)
        {
            var existingClass = await _dbContext.Classes.FindAsync(updatedClassDto.Id);
            if (existingClass == null)
            {
                return null;
            }

            existingClass.ClassName = updatedClassDto.ClassName;
            existingClass.ClassDescription = updatedClassDto.ClassDescription;
            existingClass.Count = updatedClassDto.Count; // Update as necessary
            existingClass.CourseId = updatedClassDto.CourseId;

            _dbContext.Classes.Update(existingClass);
            await _dbContext.SaveChangesAsync();

            return new ClassDto
            {
                Id = existingClass.Id,
                ClassName = existingClass.ClassName,
                ClassDescription = existingClass.ClassDescription,
                Count = existingClass.Count,
                CourseId = existingClass.CourseId
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var classToDelete = await _dbContext.Classes.FindAsync(id);
            if (classToDelete == null)
            {
                return false;
            }

            _dbContext.Classes.Remove(classToDelete);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ClassDto>> GetByCourseIdAsync(Guid courseId)
        {
            return await _dbContext.Classes
                .Where(c => c.CourseId == courseId)
                .Select(c => new ClassDto
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    ClassDescription = c.ClassDescription,
                    Count = c.Count,
                    CourseId = c.CourseId
                })
                .ToListAsync();
        }
    }
}
