﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Model.Live
{
    public class User_TicketModel
    {
        public Guid Id { get; set; }
        public Guid? TicketId { get; set; }
        public DateTime? CreateDate { get; set; }
        public String UserId { get; set; }
    }
}
