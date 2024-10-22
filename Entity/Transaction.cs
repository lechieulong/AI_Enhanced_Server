﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public String UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public decimal Amount { get; set; }

        public string PaymentStatus { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
