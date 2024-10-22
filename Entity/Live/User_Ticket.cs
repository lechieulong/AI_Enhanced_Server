using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Live
{
    public class User_Ticket
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? TicketId { get; set; }
        public DateTime CreateDate { get; set; }
        public String UserId { get; set; }

        // Navigation properties
        [ForeignKey("TicketId")]
        public Ticket? Ticket { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}
