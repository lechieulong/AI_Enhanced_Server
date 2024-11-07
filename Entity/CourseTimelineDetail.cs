using System;
using System.Text.Json.Serialization;
using Entity.CourseFolder;
namespace Entity
{
    public class CourseTimelineDetail
    {
        public Guid Id { get; set; }

        // Khóa ngoại liên kết với CourseTimeline
        public Guid CourseTimelineId { get; set; } // Sửa tên biến để phản ánh rõ ràng mối quan hệ

        [JsonIgnore]
        public CourseTimeline CourseTimeline { get; set; } // Khóa ngoại liên kết với CourseTimeline

        public string Title { get; set; }

        public string VideoUrl { get; set; }

        public string Topic { get; set; }
        public bool IsEnabled { get; set; } = true; // Mặc định là true (enabled)
    }
}