using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class PartRequestModel
    {
        public int PartNumber { get; set; }
        public string ContentText { get; set; }
        public List<QuestionTypePartRequestModel> QuestionTypeParts { get; set; } = new List<QuestionTypePartRequestModel>();
    }
}
