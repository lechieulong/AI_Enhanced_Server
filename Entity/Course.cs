using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entity
{
    public class Course
    {
        [Key] // Thêm thuộc tính khóa chính
        public Guid Id { get; set; }

        [Required] // Thêm yêu cầu cho tên khóa học
        public string CourseName { get; set; }

        public string Content { get; set; }
        public int Hours { get; set; }
        public int Days { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [JsonIgnore]
        public ICollection<CourseTimeline>? CourseTimelines { get; set; }

        [JsonIgnore]
        public ICollection<Class>? Classes { get; set; }

        public string? ImageUrl { get; set; }

        public string? UserId { get; set; } // Đảm bảo thuộc tính này có thể null

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        // Thêm thuộc tính để lưu danh sách UserCourse
        [JsonIgnore]
        public ICollection<Enrollment>? Enrollments { get; set; }
    }
}
