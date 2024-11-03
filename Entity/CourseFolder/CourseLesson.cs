using Entity.CourseFolder;
using System;
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

        [Required]
        public string ContentType { get; set; }

        public string ContentUrl { get; set; }

        public int Order { get; set; }
    }
}
