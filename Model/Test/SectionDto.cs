using Common;
using Microsoft.AspNetCore.Http;

namespace Model.Test
{
    public class SectionDto
    {
        public string SectionGuide { get; set; }
        public int SectionType { get; set; }
        public string? Image { get; set; }
        public string? Summary { get; set; }
        public string SectionContext { get; set; }
        public string? Explain { get; set; }

        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }
}
