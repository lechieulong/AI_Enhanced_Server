using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entity.CourseFolder
{
    public class Course
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string CourseName { get; set; }

        public string Content { get; set; }
        public int Hours { get; set; }
        public int Days { get; set; }

        [NotMapped]
        public List<string> Categories { get; set; } = new List<string>();

        public double Price { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        [JsonIgnore]
        public ICollection<Class> Classes { get; set; } = new List<Class>();

        public string? ImageUrl { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public bool IsEnabled { get; set; } = true;

        [JsonIgnore]
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        [JsonIgnore]
        public ICollection<CourseRating> CourseRatings { get; set; } = new List<CourseRating>();
    }
}
