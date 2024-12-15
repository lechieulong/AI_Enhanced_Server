using Entity.CourseFolder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity
{
    public class Class
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string ClassName { get; set; }

        public string ClassDescription { get; set; }
        public int Count { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; }

        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsEnabled { get; set; } = true;

        public ICollection<Enrollment>? Enrollments { get; set; }
        public int EnrollmentCount { get; set; } = 0;

        public ICollection<ClassFile>? ClassFiles { get; set; }

        [NotMapped]
        public double Price { get; set; }

        [NotMapped]
        public string UserId { get; set; }

    }
}
