namespace Entity.Test
{
    public class Answer
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public string AnswerText { get; set; }
        public int IsCorrect { get; set; }
        public Question Question { get; set; }
    }
}
