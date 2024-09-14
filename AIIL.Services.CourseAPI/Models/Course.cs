using AIIL.Services.CourseAPI.Models.Dto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIIL.Services.CourseAPI.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        [NotMapped]
        public CategoryDto Category { get; set; }
        public int MentorId { get; set; }
        [NotMapped]
        public MentorDto Mentor { get; set; }
    }

}
