using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Live
{
    public class LiveStream
    {
        [Key]
        public Guid Id { get; set; }
        public String UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public ICollection<StreamSession>? StreamSessions { get; set; }
        public ICollection<Ticket>? Tickets { get; set; }
    }
}
