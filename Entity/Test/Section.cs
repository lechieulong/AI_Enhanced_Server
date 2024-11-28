namespace Entity.Test
{
    public class Section
    {
        public Guid Id { get; set; } 
        public string? Image { get; set; }
        public string SectionGuide { get; set; } 
        public int SectionType { get; set; }
        public string SectionContext { get; set; }
        public string Explain { get; set; }
        public Part Part { get; set; } 
        public ICollection<SectionQuestion> SectionQuestions { get; set; } // Collection of questions
    }

}
