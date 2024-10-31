using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Entity;

public class Course
{
    [Key] // Thuộc tính khóa chính
    public Guid Id { get; set; }

    [Required] // Yêu cầu cho tên khóa học
    public string CourseName { get; set; }

    public string Content { get; set; }
    public int Hours { get; set; }
    public int Days { get; set; }

    // Change this property to a list of skills
    public List<string> Categories { get; set; } = new List<string>(); // Allow multiple categories

    public double Price { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [JsonIgnore]
    public ICollection<CourseTimeline>? CourseTimelines { get; set; }

    [JsonIgnore]
    public ICollection<Class>? Classes { get; set; }

    public string? ImageUrl { get; set; }

    public string? UserId { get; set; } // Đảm bảo thuộc tính này có thể null

    [ForeignKey("UserId")]
    public ApplicationUser? User { get; set; }

    // Thuộc tính cho phép enable hoặc disable course
    public bool IsEnabled { get; set; } = true; // Mặc định là true (enabled)

    [JsonIgnore]
    public ICollection<Enrollment>? Enrollments { get; set; }
}
