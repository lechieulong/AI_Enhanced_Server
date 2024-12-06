using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class UserAnswersDto
    {
        public int Skill { get; set; }
        public Guid QuestionId { get; set; }
        public Guid SkillId { get; set; }
        public int SectionType { get; set; }
        public string? SectionContext { get; set; }
        public string? Explain { get; set; }
        public Guid? PartId { get; set; }
        public string? OverallScore { get; set; }
        public List<AnswerDto> Answers { get; set; }
    }
}
