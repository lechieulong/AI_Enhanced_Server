namespace Entity.Test
{
    public class Part
    {
        public Guid Id { get; set; }
        public int PartNumber { get; set; }
        public string ContentText { get; set; }
        public string? Audio { get; set; }
        public ICollection<Section> Sections { get; set; } = new List<Section>();
        public Guid SkillId { get; set; }
        public Skill Skill { get; set; }
    }
}
