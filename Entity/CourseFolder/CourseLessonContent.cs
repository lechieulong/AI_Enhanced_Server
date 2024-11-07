using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.CourseFolder
{
    public class CourseLessonContent
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CourseLessonId { get; set; }

        [ForeignKey("CourseLessonId")]
        public CourseLesson CourseLesson { get; set; }

        [Required]
        public string ContentType { get; set; }

        [Column(TypeName = "text")]
        public string? ContentText { get; set; }

        public string? ContentUrl { get; set; }

        public int Order { get; set; }
    }
}
