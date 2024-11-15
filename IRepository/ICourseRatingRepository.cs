using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.CourseFolder;

namespace IRepository
{
    public interface ICourseRatingRepository
    {
        Task<bool> UserHasEnrolledAsync(Guid courseId, string userId);
        Task<bool> UserHasRatedCourseAsync(Guid courseId, string userId);
        Task AddRatingAsync(CourseRating rating);
        Task<IEnumerable<CourseRating>> GetCourseRatingsAsync(Guid courseId);
    }
}
