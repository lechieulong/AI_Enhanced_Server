using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Entity.CourseFolder;
namespace Entity
{
    public class Enrollment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        [ForeignKey("CourseId")]
        [JsonIgnore]
        public Course Course { get; set; }
        public Guid ClassId { get; set; } 

        [ForeignKey("ClassId")]
        [JsonIgnore]
        public Class Class { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        [JsonIgnore]
        public ApplicationUser User { get; set; }

        public string EnrollAt { get; set; }
    }
}
