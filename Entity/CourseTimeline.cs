using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Entity
{
    public class CourseTimeline
    {
        public Guid Id { get; set; }

        // Foreign key linking to the Course table
        public Guid CourseId { get; set; }

        // Ignore Course property when creating CourseTimeline
        [JsonIgnore]
        public Course? Course { get; set; } // Optional, not required during creation

        public DateTime EventDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }

        [JsonIgnore]
        public ICollection<CourseTimelineDetail>? CourseTimelineDetails { get; set; }
    }
}
