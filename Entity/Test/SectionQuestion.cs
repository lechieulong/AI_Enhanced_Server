namespace Entity.Test
{
    public class SectionQuestion
    {
        public Guid Id { get; set; }
        public Guid SectionId { get; set; } // Foreign key referencing Section
        public Section Section { get; set; } // Navigation property for Section

        public Guid QuestionId { get; set; } // Foreign key referencing Question
        public Question Question { get; set; } // Navigation property for Question
    }

}
