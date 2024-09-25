using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Test
{
    public class Question
    {
        public Guid Id { get; set; }
        public Guid TypePartId { get; set; }
        public string QuestionName { get; set; }
        public int MaxMarks { get; set; }
        public QuestionTypePart QuestionTypePart { get; set; }
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }

}
