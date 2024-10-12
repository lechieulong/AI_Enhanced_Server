using Entity;
using System.ComponentModel.DataAnnotations.Schema;

public class TeacherAvailableSchedule
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal Price { get; set; }
    public string Link { get; set; }
    public string TeacherId { get; set; }

    [ForeignKey("TeacherId")]
    public ApplicationUser Teacher { get; set; }

    public bool IsBooked { get; set; }
    public BookedTeacherSession? BookedTeacherSession { get; set; }

    //// Khóa ngoại đến Class
    public Guid ClassId { get; set; } // <-- Thêm dòng này
    //[ForeignKey("ClassId")]
    public Class Class { get; set; } // <-- Thêm dòng này
}
