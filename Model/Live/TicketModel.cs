using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Live
{
    public class TicketModel
    {
        public string SubjectName { get; set; }
        public Guid LiveStreamId { get; set; }
        public string Price { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
