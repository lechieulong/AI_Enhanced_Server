﻿using System;

namespace Entity
{
    public class CourseTimelineDetail
    {
        public Guid Id { get; set; }

        // Khóa ngoại liên kết với CourseTimeline
        public Guid CourseTimelineId { get; set; } // Sửa tên biến để phản ánh rõ ràng mối quan hệ
        public CourseTimeline CourseTimeline { get; set; } // Khóa ngoại liên kết với CourseTimeline

        public string Title { get; set; }

        public string VideoUrl { get; set; }

        public string Topic { get; set; }
    }
}
