using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Payment
{
    public class AccountBalance
    {
        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; }

        public decimal Balance { get; set; }

        public DateTime LastUpdated { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public ICollection<Balance_History>? History { get; set; }
    }
}
