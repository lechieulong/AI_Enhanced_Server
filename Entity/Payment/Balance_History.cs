using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Payment
{
    public class Balance_History
    {
        [Key]
        public Guid Id { get; set; }

        public Guid AccountBalanceId { get; set; }

        public decimal amount {  get; set; }

        public string Description {  get; set; }
        public string Type { get; set; }

        public DateTime CreateDate { get; set; }
        [ForeignKey("AccountBalanceId")]
        public AccountBalance? AccountBalance { get; set; }
    }
}
