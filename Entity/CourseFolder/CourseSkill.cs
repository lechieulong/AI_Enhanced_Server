using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entity.CourseFolder
{
    public class CourseSkill
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        [ForeignKey("CourseId")]
        [JsonIgnore]
        public Course Course { get; set; }

        [Required]
        public string Type { get; set; }

        public string Description { get; set; }

        [JsonIgnore]
        public ICollection<CoursePart> CourseParts { get; set; } = new List<CoursePart>();
    }
}
