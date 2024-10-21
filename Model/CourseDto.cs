using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Model
{
    public class CourseDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }

        public string CourseName { get; set; }
        public string Content { get; set; }
        public int Hours { get; set; }
        public int Days { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [JsonIgnore] // Bỏ qua khi tạo Course
        public ICollection<CourseTimelineDto>? CourseTimelines { get; set; }

        [JsonIgnore] // Bỏ qua khi tạo Course
        public ICollection<UserDto>? Users { get; set; }
    }
}
