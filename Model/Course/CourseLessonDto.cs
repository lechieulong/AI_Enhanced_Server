using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Course
{
    public class CourseLessonDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<CourseLessonContentDto> Contents { get; set; }
    }
}
