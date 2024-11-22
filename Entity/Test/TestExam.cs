namespace Entity.Test
{
    public class TestExam
    {
        public Guid Id { get; set; }
        public string TestName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public Guid LessonId { get; set; }
        public Guid ClassId { get; set; }
        public Guid UserID { get; set; }
        public ICollection<Skill> SkillTests { get; set; } = new List<Skill>();
        public int TestCreateBy { get; set; }
    }
}
