using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class PartResponseModel
    {
        public int PartNumber { get; set; }
        public string ContentText { get; set; }
        public List<QuestionTypePartResponseModel> QuestionTypeParts { get; set; } = new List<QuestionTypePartResponseModel>();
    }
}
