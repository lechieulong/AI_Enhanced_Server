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
        public string QuestionName { get; set; }
        public int QuestionType { get; set; } = 0;
        public int? Skill {  get; set; }
        public int? PartNumber { get; set; }
        public string Explain { get; set; }
        public Guid UserId { get; set; }
        public ICollection<SectionQuestion> SectionQuestions { get; set; } 
        public ICollection<Answer> Answers { get; set; }
    }

}
