    using Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Model.Test
    {
        public class QuestionResponse
        {
            public Guid Id { get; set; }
            public string QuestionName { get; set; }
            public QuestionTypeENum QuestionType { get; set; }
            public int? Skill { get; set; }       // Add Skill
            public int? PartNumber { get; set; }  // Add PartNumber
            public List<AnswerResponse> Answers { get; set; } // Add Answers model for proper mapping
        }
    }
