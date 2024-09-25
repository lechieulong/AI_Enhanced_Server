namespace Entity.Test
{
    public class QuestionTypePart
    {
        public Guid Id { get; set; }
        public Guid PartId { get; set; }
        public string QuestionGuide { get; set; }
        public int QuestionType { get; set; }
        public PartSkill PartSkill { get; set; }
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
