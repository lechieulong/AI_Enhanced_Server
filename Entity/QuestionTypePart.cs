using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class QuestionTypePart
    {
        public Guid Id { get; set; }
        public Guid PartId { get; set; }
        public string QuestionGuide { get; set; }
        public string QuestionType { get; set; }

        public PartSkill PartSkill { get; set; }
        public ICollection<Question> Questions { get; set; } = new List<Question>();

    }
}
