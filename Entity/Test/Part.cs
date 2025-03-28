﻿namespace Entity.Test
{
    public class Part
    {
        public Guid Id { get; set; }
        public int PartNumber { get; set; }
        public string? ContentText { get; set; }
        public string? Audio { get; set; }
        public string? Image { get; set; }
        public string? ScriptAudio { get; set; }
        public int AudioProcessingStatus { get; set; }
        public ICollection<Section> Sections { get; set; } = new List<Section>();
        public Skill Skill { get; set; }
    }
}
