using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class AnswerRequest
    {
        public string AnswerFilling { get; set; }
        public int AnswerTrueFalse { get; set; }
        public List<AnswerMatchingRequest> AnswerMatchingRequest { get; set; } = new List<AnswerMatchingRequest>();
        public List<AnswerOptionsRequest> AnswerOptionsRequest { get; set; } = new List<AnswerOptionsRequest>();

    }
}
