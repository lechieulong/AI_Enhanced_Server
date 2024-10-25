namespace Entity.Test
{
    public class Skill
    {
        public Guid Id { get; set; }
        public int Type { get; set; }
        public int Duration { get; set; }
        public ICollection<Part> Parts { get; set; } = new List<Part>();
        public Guid TestId { get; set; }
        public TestExam Test { get; set; }
    }
}
