namespace Entity.Test
{
    public class SectionQuestion
    {
        public Guid Id { get; set; }
        public Section Section { get; set; } // Navigation property for Section
        public Question Question { get; set; } // Navigation property for Question
    }

}
