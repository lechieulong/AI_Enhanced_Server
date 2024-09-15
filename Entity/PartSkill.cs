using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Entity
{
    public class PartSkill
    {
        public Guid Id { get; set; }
        public Guid TestId { get; set; }
        public int PartNumber { get; set; }
        public int SkillTest { get; set; }
        public string ContentText { get; set; }
        public string AudioURL { get; set; }

        public TestExam Test { get; set; }
        public ICollection<QuestionTypePart> QuestionTypeParts { get; set; } = new List<QuestionTypePart>();
    }

}