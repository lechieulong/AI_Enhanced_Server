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
        public Course? Course { get; set; }

        // Thay đổi ClassId thành Nullable Guid
        public Guid? ClassId { get; set; } // Cho phép null

        [ForeignKey("ClassId")]
        [JsonIgnore]
        public Class? Class { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        [JsonIgnore]
        public ApplicationUser? User { get; set; }
    }
}
