using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class SkillRequest
    {
        public SkillTypeEnum SKillType { get; set; }
        public int Duration { get; set; }
        public List<PartRequestModel> QuestionTypeParts { get; set; } = new List<QuestionTypePartRequestModel>();
    }
}
