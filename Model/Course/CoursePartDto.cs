using System;
using System.Collections.Generic;

namespace Model.Course
{
    public class CoursePartDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public List<CourseLessonDto> CourseLessons { get; set; }
    }
}
