using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CourseTimelineDetailDto
    {
        public Guid CourseTimelineId { get; set; }
        public string Title { get; set; }
        public string VideoUrl { get; set; }
        public string Topic { get; set; }
        public bool IsEnabled { get; set; } = true; // Mặc định là true (enabled)

    }
}
