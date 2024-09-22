using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class AnswerOptionsResponse
    {
        public int PartNumber { get; set; }
        public int SkillTest { get; set; }
        public string ContentText { get; set; }
        public string AudioUrl { get; set; }
        public List<QuestionTypePartRequestModel> QuestionTypeParts { get; set; } = new List<QuestionTypePartRequestModel>();
    }
}
