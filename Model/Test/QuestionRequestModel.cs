using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class QuestionRequestModel
    {
        public string QuestionName { get; set; }
        public int MaxMarks { get; set; }
        public List<Answe> Questions { get; set; } = new List<QuestionRequestModel>();

    }
}
