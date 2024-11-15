using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class UserAnswersPayloadDto
    {
        public Dictionary<string, UserAnswersDto> UserAnswers { get; set; }
    }
}
