using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class QuestionDto
    {
        public Guid QuestionId { get; set; }
        public string? QuestionName { get; set; }
        public string? Explain { get; set; }
        public int QuestionType { get; set; }
        public List<AnswerDto> Answers { get; set; } = new List<AnswerDto>();

    }
}
