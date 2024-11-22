using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class AnswerDto
    {
        public Guid? AnswerId { get; set; }
        public string? AnswerText { get; set; }
        public TypeCorrect? IsCorrect { get; set; }
     }
}
