using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Entity.CourseFolder
{
    public class CourseTimeline
    {
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }

        public int Order { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public bool IsEnabled { get; set; } = true;

        [JsonIgnore]
        public Course? Course { get; set; }

        [JsonIgnore]
        public ICollection<CourseTimelineDetail>? CourseTimelineDetails { get; set; }
    }
}
