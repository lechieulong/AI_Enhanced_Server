﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Test
{
    public class TestResult
    {
        public Guid Id { get; set; } 
        public Guid TestId { get; set; }
        public Guid UserId { get; set; }
        public int SkillType { get; set; } 
        public Decimal Score { get; set; }
        public int NumberOfCorrect { get; set; } 
        public int TotalQuestion { get; set; }
        public DateTime TestDate { get; set; }
        public int TimeMinutesTaken { get; set; }   
        public int SecondMinutesTaken { get; set; }
        public int AttemptNumber { get; set; }

    }
}
