using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Model
{
    public class CourseDto
    {
        public string UserId { get; set; }
        public string CourseName { get; set; }
        public string Content { get; set; }
        public int Hours { get; set; }
        public int Days { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public bool IsEnabled { get; set; } = true; // Thêm thuộc tính IsEnabled

        [JsonIgnore] // Bỏ qua khi tạo Course
        public ICollection<CourseTimelineDto>? CourseTimelines { get; set; }

        [JsonIgnore] // Bỏ qua khi tạo Course
        public ICollection<UserDto>? Users { get; set; }
    }
}
