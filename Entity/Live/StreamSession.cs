using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Live
{
    public class StreamSession
    {
        [Key]
        public Guid Id { get; set; }
        public int Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Type { get; set; }
        public Guid LiveStreamId { get; set; }

        // Navigation property
        [ForeignKey("LiveStreamId")]
        public LiveStream? LiveStream { get; set; }
    }
}
