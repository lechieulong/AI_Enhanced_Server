using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class QuestionModel
    {
        public string QuestionName { get; set; }
        public QuestionTypeENum QuestionType { get; set; }
        public int Skill { get; set; }
        public int Part { get; set; }
        public List<AnswerModel> Answers { get; set; }

    }
}
