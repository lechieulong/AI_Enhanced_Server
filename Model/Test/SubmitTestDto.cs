using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace Model.Test
    {
        public class SubmitTestDto
        {
            public Dictionary<string, UserAnswersDto> UserAnswers { get; set; }
            public int TimeMinutesTaken { get; set; }  // Time in minutes
            public int TimeSecondsTaken { get; set; }  // Time in seconds
            public int TotalQuestions { get; set; }
        }
    }
