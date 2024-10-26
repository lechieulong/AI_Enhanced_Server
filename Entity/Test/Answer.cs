namespace Entity.Test
{
    public class Answer
    {
        public Guid Id { get; set; }
        public string? AnswerText { get; set; }
        public int? TypeCorrect { get; set; }
        public Question Question { get; set; }
        public Guid QuestionId { get; set; }

    }
}
