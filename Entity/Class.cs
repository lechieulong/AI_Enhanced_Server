using Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Class
{
    [Key]
    public Guid Id { get; set; }

    public string ClassName { get; set; }
    public string ClassDescription { get; set; }
    public int Count { get; set; }

    public Guid CourseId { get; set; }

    [ForeignKey("CourseId")]
    public Course Course { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; } // Thêm thuộc tính EndDate

    public TimeSpan StartTime { get; set; } // Thời gian bắt đầu lớp học
    public TimeSpan EndTime { get; set; } // Thời gian kết thúc lớp học

    public string? ImageUrl { get; set; }

    // Quan hệ nhiều-nhiều với User thông qua UserClass
    public ICollection<Enrollment>? Enrollments { get; set; }
}
