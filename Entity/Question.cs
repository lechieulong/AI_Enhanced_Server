using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Question
    {
        public Guid Id { get; set; }
        public Guid TypePartId { get; set; }
        public string QuestionName { get; set; }
        public string Answer { get; set; }
        public int MaxMarks { get; set; }
        public QuestionTypePart QuestionTypePart { get; set; }
    }

}
