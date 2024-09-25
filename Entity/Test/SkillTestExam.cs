namespace Entity.Test
{
    public class SkillTestExam
    {
        public Guid Id { get; set; }
        public Guid TestId { get; set; }
        public int SkillType { get; set; }
        public int Duration { get; set; }
        public TestExam Test { get; set; }
        public ICollection<PartSkill> PartSkills { get; set; } = new List<PartSkill>();
    }
}
