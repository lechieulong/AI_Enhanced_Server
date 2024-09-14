using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AIIL.Services.CourseAPI.Models.Dto
{
    public class CourseDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public CategoryDto? Category { get; set; }
        public int MentorId { get; set; }
        public MentorDto? Mentor { get; set; }
    }
}
