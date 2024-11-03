using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entity.CourseFolder
{
    public class CoursePart
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid SkillId { get; set; }

        [ForeignKey("SkillId")]
        [JsonIgnore]
        public CourseSkill Skill { get; set; }

        [Required]
        public int PartNumber { get; set; }

        [Required]
        public string Title { get; set; }

        public string ContentText { get; set; }

        public string Audio { get; set; }

        public string Image { get; set; }

        public string VideoUrl { get; set; }

        [JsonIgnore]
        public ICollection<CourseLesson> CourseLessons { get; set; } = new List<CourseLesson>();
    }
}
