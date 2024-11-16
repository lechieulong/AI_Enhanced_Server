using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Course
{
    public class CoursePartDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; }
        public string ContentUrl { get; set; }
        public int Order { get; set; }
        public List<CourseLessonDto> CourseLessons { get; set; }
    }

}
