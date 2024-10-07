using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class SectionModel
    {
        public Guid Id { get; set; }
        public string QuestionGuide { get; set; }
        public QuestionTypeENum QuestionType { get; set; }
        public string Image { get; set; }
        public List<QuestionModel> Questions { get; set; } = new List<QuestionModel>();
    }
}
