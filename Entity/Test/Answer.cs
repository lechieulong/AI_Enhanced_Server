namespace Entity.Test
{
    public class Answer
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public string AnswerFilling { get; set; }
        public int AnswerTrueFalse { get; set; }
        public Question Question { get; set; }
        public ICollection<AnswerOptions> AnswerOptions { get; set; } = new List<AnswerOptions>();
        public ICollection<AnswerMatching> AnswerMatchings { get; set; } = new List<AnswerMatching>();
    }
}
