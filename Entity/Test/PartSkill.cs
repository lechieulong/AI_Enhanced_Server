namespace Entity.Test
{
    public class PartSkill
    {
        public Guid Id { get; set; }
        public Guid SkillId { get; set; }
        public int PartNumber { get; set; }
        public string ContentText { get; set; }
        public SkillTestExam Skill { get; set; }
        public ICollection<QuestionTypePart> QuestionTypeParts { get; set; } = new List<QuestionTypePart>();
    }
}
