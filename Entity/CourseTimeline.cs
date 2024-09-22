using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Entity
{
    public class CourseTimeline
    {
        public int Id { get; set; }

        // Khóa ngoại liên kết với bảng Course
        public int CourseId { get; set; }

        // Bỏ qua thuộc tính Course khi tạo CourseTimeline
        [JsonIgnore]
        public Course? Course { get; set; } // Có thể giữ lại nhưng không yêu cầu trong việc tạo

        public DateTime EventDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }

        [JsonIgnore]
        public ICollection<CourseTimelineDetail>? CourseTimelineDetails { get; set; }
    }
}
