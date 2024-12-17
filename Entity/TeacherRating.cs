using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.TeacherFolder
{
    public class TeacherRating
    {
        [Key]
        public Guid Id { get; set; } // Khóa chính

        [Required]
        public string UserId { get; set; } // ID của teacher được đánh giá

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } // Liên kết với ApplicationUser

        public string LearnerID { get; set; } // ID của learner được đánh giá

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int RatingValue { get; set; } // Điểm đánh giá (1 - 5)

        public string Review { get; set; } // Nội dung bình luận từ người dùng (tùy chọn)

        public string RatedAt { get; set; } // Thời gian đánh giá
    }
}
