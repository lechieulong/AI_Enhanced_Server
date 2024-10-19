using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Model
{
    public class CourseTimelineDto
    {

        // Khóa ngoại liên kết với bảng Course
        public Guid CourseId { get; set; }

        // Bỏ qua thuộc tính Course khi tạo CourseTimeline
        [JsonIgnore]
        public CourseDto? Course { get; set; } // Có thể giữ lại nhưng không yêu cầu trong việc tạo

        public DateTime EventDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }

        [JsonIgnore]
        public ICollection<CourseTimelineDetailDto>? CourseTimelineDetails { get; set; }
    }
}
