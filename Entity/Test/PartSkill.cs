namespace Entity.Test
{
    public class PartSkill
    {
        public Guid Id { get; set; }
        public Guid SkillId { get; set; }
        public int PartNumber { get; set; }
        public string ContentText { get; set; }
        public string Audio { get; set; }
        public string Image { get; set; }
        public SkillTestExam Skill { get; set; }
        public ICollection<Section> QuestionTypeParts { get; set; } = new List<Section>();
    }
}
