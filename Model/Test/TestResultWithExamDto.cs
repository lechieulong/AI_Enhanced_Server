using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class TestResultWithExamDto
    {
        public Guid Id { get; set; }
        public Guid TestId { get; set; }
        public Guid UserId { get; set; }
        public int SkillType { get; set; }
        public Guid SkillId { get; set; }
        public decimal Score { get; set; }
        public int NumberOfCorrect { get; set; }
        public int TotalQuestion { get; set; }
        public DateTime TestDate { get; set; }
        public int TimeMinutesTaken { get; set; }
        public int SecondMinutesTaken { get; set; }
        public int AttemptNumber { get; set; }
        public string TestName { get; set; } 
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public List<int> TotalParts { get; set; }
    }

}
