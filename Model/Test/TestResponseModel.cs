using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class TestResponseModel
    {
        public Guid Id { get; set; }
        public string TestName { get; set; }
        public List<Guid> ClassIds { get; set; } = new List<Guid>();
        public int Duration { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public List<SkillResponse> Parts { get; set; } = new List<SkillResponse>();
    }
}
