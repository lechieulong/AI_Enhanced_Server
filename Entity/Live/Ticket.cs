using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Live
{
    public class Ticket
    {
        [Key]
        public Guid Id { get; set; }
        public string SubjectName { get; set; }
        public Guid LiveStreamId { get; set; }
        public string Price { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Navigation property to LiveStream
        [ForeignKey("LiveStreamId")]
        public LiveStream? LiveStream { get; set; }
        public ICollection<User_Ticket>? User_Tickets { get; set; }
    }
}
