using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class QuestionBankModel
    {
        public Guid? QuestionId { get; set; }
        public string QuestionName { get; set; }
        public int QuestionType { get; set; }
        public int SkillType {  get; set; }
        public int Part {  get; set; }
        public List<AnswerDto> Answers { get; set; } = new List<AnswerDto>();

    }
}
