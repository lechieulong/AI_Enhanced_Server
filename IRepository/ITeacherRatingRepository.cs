using Entity;
using Entity.TeacherFolder;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository
{
    public interface ITeacherRatingRepository
    {
        // Thêm một rating mới
        Task CreateRatingAsync(TeacherRating rating);
        Task<IEnumerable<TopRatedTeacherDto>> GetTopRatedTeachersAsync();

        Task<IEnumerable<TeacherRating>> GetAllRatingsAsync();
        Task<bool> HasLearnerRatedTeacherAsync(string userId, string learnerId);
    }
}
