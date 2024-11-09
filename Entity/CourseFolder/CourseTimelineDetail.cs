using System;
using System.Text.Json.Serialization;

namespace Entity.CourseFolder
{
    public class CourseTimelineDetail
    {
        public Guid Id { get; set; }

        public Guid CourseTimelineId { get; set; }

        public string? Title { get; set; }

        public string? VideoUrl { get; set; }

        public string? Topic { get; set; }

        public string Skill { get; set; }
        public bool IsEnabled { get; set; } = true;
        [JsonIgnore]
        public CourseTimeline CourseTimeline { get; set; }
    }
}
