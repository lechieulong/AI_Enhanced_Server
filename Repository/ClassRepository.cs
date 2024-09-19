using IRepository;
using Model;
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
            // Map List<Class> to List<ClassDto>
            return await _dbContext.Classes
                .Select(c => new ClassDto
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    ClassDescription = c.ClassDescription,
                    Count = c.Count,
                    CourseId = c.CourseId
                }).ToListAsync();
        }
        public async Task<ClassDto> GetByIdAsync(int id)
        {
            var classEntity = await _dbContext.Classes.FindAsync(id);
            if (classEntity == null)
            {
                return null;
            }

            // Map Class to ClassDto
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
            // Map ClassDto to Class
            var classEntity = new Class
            {
                ClassName = newClassDto.ClassName,
                ClassDescription = newClassDto.ClassDescription,
                Count = newClassDto.Count,
                CourseId = newClassDto.CourseId
            };

            await _dbContext.Classes.AddAsync(classEntity);
            await _dbContext.SaveChangesAsync();

            // Map Class back to ClassDto
            newClassDto.Id = classEntity.Id;
            return newClassDto;
        }
        public async Task<ClassDto> UpdateAsync(ClassDto updatedClassDto)
        {
            var existingClass = await _dbContext.Classes.FindAsync(updatedClassDto.Id);
            if (existingClass == null)
            {
                return null;
            }

            // Update properties
            existingClass.ClassName = updatedClassDto.ClassName;
            existingClass.ClassDescription = updatedClassDto.ClassDescription;
            existingClass.Count = updatedClassDto.Count;
            existingClass.CourseId = updatedClassDto.CourseId;

            _dbContext.Classes.Update(existingClass);
            await _dbContext.SaveChangesAsync();

            // Map Class back to ClassDto
            return new ClassDto
            {
                Id = existingClass.Id,
                ClassName = existingClass.ClassName,
                ClassDescription = existingClass.ClassDescription,
                Count = existingClass.Count,
                CourseId = existingClass.CourseId
            };
        }
        public async Task<bool> DeleteAsync(int id)
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

    }
}
