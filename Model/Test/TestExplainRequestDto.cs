using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class TestExplainRequestDto
    {
        public Guid TestId { get; set; }
        public Guid UserId { get; set; }
        public Guid? SkillId { get; set; }
        public List<int> TotalPartsSubmit {  get; set; }

    }
}
