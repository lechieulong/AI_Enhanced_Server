using AutoMapper;
using Entity;
using Entity.CourseFolder;
using Entity.Data;
using IRepository;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using Entity.CourseFolder;
using System.Linq;
using System.Threading.Tasks;

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
                    ClassName = c.ClassName,
                    ClassDescription = c.ClassDescription,
                    CourseId = c.CourseId,
                    IsEnabled = c.IsEnabled
                }).ToListAsync();
        }

        public async Task UpdateClassEnabledStatusAsync(Guid classId, bool isEnabled)
        {
            var classEntity = await _dbContext.Classes.FindAsync(classId);
            if (classEntity != null)
            {
                classEntity.IsEnabled = isEnabled;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<ClassDto> CreateAsync(Class newClassDto)
        {
            if (newClassDto == null)
            {
                throw new ArgumentNullException(nameof(newClassDto), "Class DTO cannot be null.");
            }

            var classEntity = new Class
            {
                Id = Guid.NewGuid(), // Tạo ID mới cho lớp học
                ClassName = newClassDto.ClassName,
                ClassDescription = newClassDto.ClassDescription,
                CourseId = newClassDto.CourseId,
                IsEnabled = newClassDto.IsEnabled,
            };

            try
            {
                await _dbContext.Classes.AddAsync(classEntity);
                await _dbContext.SaveChangesAsync();

                // Trả về ClassDto tương ứng với lớp học vừa được tạo
                return new ClassDto
                {
                    ClassName = classEntity.ClassName,
                    ClassDescription = classEntity.ClassDescription,
                    CourseId = classEntity.CourseId,
                    IsEnabled = classEntity.IsEnabled
                };
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while creating the class.", ex);
            }
        }

        public async Task<Enrollment> GetEnrollmentAsync(Guid courseId, string userId)
        {
            return await _dbContext.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == userId);
        }

        public async Task<Class> GetEntityByIdAsync(Guid classId)
        {
            return await _dbContext.Classes.FirstOrDefaultAsync(c => c.Id == classId);
        }



        public async Task<ClassDto> UpdateAsync(Guid classId, ClassDto updatedClassDto)
        {
            var existingClass = await _dbContext.Classes.FindAsync(classId);
            if (existingClass == null)
            {
                return null;
            }

            // Cập nhật các thuộc tính
            existingClass.ClassName = updatedClassDto.ClassName;
            existingClass.ClassDescription = updatedClassDto.ClassDescription;
            existingClass.CourseId = updatedClassDto.CourseId;
            existingClass.IsEnabled = updatedClassDto.IsEnabled;

            _dbContext.Classes.Update(existingClass);
            await _dbContext.SaveChangesAsync();

            return new ClassDto
            {
                ClassName = existingClass.ClassName,
                ClassDescription = existingClass.ClassDescription,
                CourseId = existingClass.CourseId,
                IsEnabled = existingClass.IsEnabled
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
                    CourseId = c.CourseId,
                    IsEnabled = c.IsEnabled,
                    EnrollmentCount = c.EnrollmentCount
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ClassDto>> GetByTeacherIdAsync(string teacherId)
        {
            return await _dbContext.Classes
                .Where(c => c.Course.UserId == teacherId)
                .Select(c => new ClassDto
                {
                    ClassName = c.ClassName,
                    ClassDescription = c.ClassDescription,
                    CourseId = c.CourseId,
                    IsEnabled = c.IsEnabled
                })
                .ToListAsync();
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
                ClassName = classEntity.ClassName,
                ClassDescription = classEntity.ClassDescription,
                CourseId = classEntity.CourseId,
                IsEnabled = classEntity.IsEnabled
            };
        }

        public async Task<List<object>> GetUnenrolledClassesAsync(Guid courseId, string userId)
        {
            // Lấy thông tin Price và UserId từ Course
            var courseInfo = await _dbContext.Courses
                .Where(c => c.Id == courseId)
                .Select(c => new { c.Price, c.UserId })
                .FirstOrDefaultAsync();

            if (courseInfo == null)
            {
                throw new Exception("Course not found.");
            }

            // Lấy danh sách Class chưa đăng ký
            var classes = await _dbContext.Classes
                .Where(c => c.CourseId == courseId && !c.Enrollments.Any(e => e.UserId == userId))
                .Select(c => new
                {
                    ClassId = c.Id,
                    ClassName = c.ClassName,
                    Price = courseInfo.Price, // Gắn giá từ Course
                    TeacherID = courseInfo.UserId // Gắn UserId từ Course
                })
                .ToListAsync();

            return classes.Cast<object>().ToList();
        }

    }
}
