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
        public Guid Id { get; set; }
        public string QuestionName { get; set; }
        public QuestionTypeENum QuestionType { get; set; }
    }
}
