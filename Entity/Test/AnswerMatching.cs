namespace Entity.Test
{
    public class AnswerMatching
    {
        public Guid Id { get; set; }
        public Guid AnswerId { get; set; }
        public string Heading { get; set; }
        public string Matching { get; set; }
        public Answer Answer { get; set; }
    }
}
