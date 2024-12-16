using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class TeacherRatingDto
    {

        public string UserId { get; set; } // ID của teacher được đánh giá

        public string LearnerID { get; set; } // ID của learner được đánh giá

        public int RatingValue { get; set; } // Điểm đánh giá (1 - 5)

        public string Review { get; set; } // Nội dung bình luận từ người dùng (tùy chọn)

        public DateTime RatedAt { get; set; } = DateTime.UtcNow; // Thời gian đánh giá
    }
}
