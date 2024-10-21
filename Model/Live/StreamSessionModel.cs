using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Live
{
    public class StreamSessionModel
    {
        public Guid Id { get; set; }
        public int Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Type { get; set; }
        public Guid LiveStreamId { get; set; }
    }
}
