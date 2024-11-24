using Entity.CourseFolder;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class CoursePart
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid CourseSkillId { get; set; }

    [ForeignKey("CourseSkillId")]
    [JsonIgnore]
    public CourseSkill Skill { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string ContentType { get; set; }

    public string ContentUrl { get; set; }

    public int Order { get; set; }

    [JsonIgnore]
    public ICollection<CourseLesson> CourseLessons { get; set; } = new List<CourseLesson>();
    public TestExam TestExam { get; set; }
}
