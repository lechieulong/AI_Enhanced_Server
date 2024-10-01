using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Entity
{
    public class Course
    {
        public int Id { get; set; }
        public string CourseName { get; set; }
        public string Content { get; set; }
        public int Hours { get; set; }
        public int Days { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [JsonIgnore] // Bỏ qua khi tạo Course
        public ICollection<CourseTimeline>? CourseTimelines { get; set; }
        public ICollection<Class>? Classes { get; set; }
        public string? ImageUrl { get; set; }
    }
}
