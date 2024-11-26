using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity;
using Entity.Data;
using Microsoft.EntityFrameworkCore;
using Entity.CourseFolder;
using IRepository;

namespace Repository
{
    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;

        public CourseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountAsync()
        {
            return await _context.Courses
                .Where(course => course.IsEnabled)
                .CountAsync();
        }

        public async Task<List<Course>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.Courses
                .Include(course => course.User)
                .Where(course => course.IsEnabled)
                .OrderBy(course => course.Id) 
                .Skip((pageNumber - 1) * pageSize) 
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Course> GetByIdAsync(Guid id)
        {
            return await _context.Courses
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task CreateAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Course course)
        {
            var existingCourse = await _context.Courses.FindAsync(course.Id);
            if (existingCourse != null)
            {
                existingCourse.CourseName = course.CourseName;
                existingCourse.Content = course.Content;
                existingCourse.Hours = course.Hours;
                existingCourse.Days = course.Days;
                existingCourse.Categories = course.Categories;
                existingCourse.Price = course.Price;
                existingCourse.IsEnabled = course.IsEnabled;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var course = await _context.Courses
                .Include(c => c.Classes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                throw new KeyNotFoundException($"Course with ID {id} not found.");

            _context.Classes.RemoveRange(course.Classes);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Course>> GetAllEnabledCoursesAsync()
        {
            return await _context.Courses.Where(c => c.IsEnabled).ToListAsync();
        }

        public async Task<List<Course>> GetAllDisabledCoursesAsync()
        {
            return await _context.Courses.Where(c => !c.IsEnabled).ToListAsync();
        }

        public async Task<List<Course>> GetAllCourseByUserIdAsync(string userId)
        {
            return await _context.Courses.Where(c => c.UserId == userId).ToListAsync();
        }

        public async Task<List<Course>> GetCreatedCourses(string userId)
        {
            return await _context.Courses
                .Include(course => course.User)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateCourseEnabledStatusAsync(Guid courseId, bool isEnabled)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course != null)
            {
                course.IsEnabled = isEnabled;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Guid?> GetCourseIdByLessonContentIdAsync(Guid courseLessonContentId)
        {
            var courseId = await (from clc in _context.CourseLessonContents
                                  join cl in _context.CourseLessons on clc.CourseLessonId equals cl.Id
                                  join cp in _context.CourseParts on cl.CoursePartId equals cp.Id
                                  join cs in _context.CourseSkills on cp.CourseSkillId equals cs.Id
                                  join c in _context.Courses on cs.CourseId equals c.Id
                                  where clc.Id == courseLessonContentId
                                  select c.Id)
                                 .FirstOrDefaultAsync();

            return courseId == Guid.Empty ? (Guid?)null : courseId;
        }

        public async Task<bool> CheckLecturerOfCourse(string userId, Guid courseId)
        {
            try
            {
                var course = await _context.Courses
                    .Where(c => c.Id == courseId)
                    .FirstOrDefaultAsync();

                return course?.UserId == userId;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task UpdateCourseRatingAsync(Guid courseId)
        {
            var course = await _context.Courses
                .Include(c => c.CourseRatings)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course != null)
            {
                var ratings = course.CourseRatings;
                if (ratings.Any())
                {
                    course.AverageRating = Math.Round(ratings.Average(r => r.RatingValue), 2);
                    course.RatingCount = ratings.Count;
                }
                else
                {
                    course.AverageRating = 0;
                    course.RatingCount = 0;
                }

                _context.Courses.Update(course);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddRatingAsync(CourseRating rating)
        {
            _context.CourseRatings.Add(rating);
            await _context.SaveChangesAsync();

            // Cập nhật trung bình cộng rating trong bảng Course
            var course = await _context.Courses.FindAsync(rating.CourseId);

            if (course != null)
            {
                var ratings = await _context.CourseRatings
                    .Where(r => r.CourseId == rating.CourseId)
                    .ToListAsync();

                course.AverageRating = Math.Round(ratings.Average(r => r.RatingValue), 2);
                course.RatingCount = ratings.Count;

                _context.Courses.Update(course);
                await _context.SaveChangesAsync();
            }
        }

    }
}
