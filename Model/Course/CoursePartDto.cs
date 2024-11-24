using System;
using System.Collections.Generic;

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
        public Guid? TestId { get; set; }
    }
}
