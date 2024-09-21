﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CourseTimelineDetailDto
    {
        public int Id { get; set; }

        // Khóa ngoại liên kết với CourseTimeline
        public int TimelineId { get; set; }
        public CourseTimelineDto CourseTimeline { get; set; }

        public string Title { get; set; }

        public string VideoUrl { get; set; }

        public string Topic { get; set; }
    }
}
