using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Test
{
    public class TestModel
    {
        public Guid Id { get; set; }
        public string TestName { get; set; }
        public List<Guid> ClassIds { get; set; } = new List<Guid>();
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public Guid SectionCourseId { get; set; }
        public Guid UserId { get; set; }

    }
}
