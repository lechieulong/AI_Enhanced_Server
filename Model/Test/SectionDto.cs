using Common;
using Microsoft.AspNetCore.Http;

namespace Model.Test
{
    public class SectionDto
    {
        public string SectionGuide { get; set; }
        public int SectionType { get; set; }
        public string? Image { get; set; }
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }
}
