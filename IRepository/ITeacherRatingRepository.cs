using Entity.TeacherFolder;
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
        Task<TeacherRating> CreateRatingAsync(TeacherRating rating);

        Task<IEnumerable<TeacherRating>> GetAllRatingsAsync();
    }
}
