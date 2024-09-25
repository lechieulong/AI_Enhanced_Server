namespace Entity.Test
{
    public class AnswerOptions
    {
        public Guid Id { get; set; }
        public Guid AnswerId { get; set; }
        public string AnswerText  { get; set; }
        public string IsCorrect { get; set; }
        public Answer Answer { get; set; }
    }
}
