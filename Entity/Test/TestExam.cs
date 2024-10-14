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
        public Guid CreateBy {  get; set; }
        public ICollection<SkillTestExam> SkillTests { get; set; } = new List<SkillTestExam>();
    }
}
