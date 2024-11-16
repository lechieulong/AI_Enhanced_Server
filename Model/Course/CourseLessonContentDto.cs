using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Course
{
    public class CourseLessonContentDto
    {
        public Guid Id { get; set; }
        public string ContentType { get; set; }
        public string? ContentText { get; set; }
        public string? ContentUrl { get; set; }
        public int Order { get; set; }
    }

}
