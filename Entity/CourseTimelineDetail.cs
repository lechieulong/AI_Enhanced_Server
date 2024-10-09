using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class CourseTimelineDetail
    {
        public Guid Id { get; set; }

        // Khóa ngoại liên kết với CourseTimeline
        public Guid TimelineId { get; set; }
        public CourseTimeline CourseTimeline { get; set; }

        public string Title { get; set; }

        public string VideoUrl { get; set; }

        public string Topic { get; set; }
    }
}
