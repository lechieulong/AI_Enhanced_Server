using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Live
{
    public class User_Gift
    {
        [Key]
        public Guid Id { get; set; }
        public String? UserId { get; set; }
        public Guid GiftId { get; set; }
        public String? ReceiverId { get; set; }
        public DateTime GiftTime { get; set; }
        public String? Message { get; set; }
        public decimal Amount { get; set; }
        [ForeignKey("GiftId")]
        public Gift? Gift { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
        [ForeignKey("ReceiverId")]
        public ApplicationUser? Receiver { get; set; }
    }
}