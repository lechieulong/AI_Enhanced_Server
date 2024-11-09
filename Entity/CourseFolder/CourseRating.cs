using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.CourseFolder
{
    public class CourseRating
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int RatingValue { get; set; } // Giá trị đánh giá (1-5 sao)

        public string Review { get; set; } // Nội dung đánh giá (tùy chọn)

        public DateTime RatedAt { get; set; } = DateTime.Now; // Ngày đánh giá
    }
}
