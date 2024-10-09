namespace Entity.Test
{
    public class Section
    {
        public Guid Id { get; set; } 
        public string Image { get; set; }
        public string SectionGuide { get; set; } 
        public string SectionType { get; set; }

        public Guid PartId { get; set; } 
        public PartSkill Part { get; set; } 
        public ICollection<SectionQuestion> SectionQuestions { get; set; } // Collection of questions
    }

}
