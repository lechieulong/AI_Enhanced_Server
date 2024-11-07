using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.CourseFolder
{
    public class CourseLesson
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CoursePartId { get; set; }

        [ForeignKey("CoursePartId")]
        public CoursePart CoursePart { get; set; }

        [Required]
        public string Title { get; set; }

        // Quan hệ một-nhiều với CourseLessonContent
        public ICollection<CourseLessonContent> Contents { get; set; } = new List<CourseLessonContent>();
    }
}
