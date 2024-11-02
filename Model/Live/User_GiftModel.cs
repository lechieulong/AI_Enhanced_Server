using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Live
{
    public class User_GiftModel
    {
        public Guid? Id { get; set; }
        public String? UserId { get; set; }
        public Guid GiftId { get; set; }
        public String? ReceiverId { get; set; }
        public DateTime? GiftTime { get; set; }
        public String? Message { get; set; }
        public decimal Amount { get; set; }
    }
}