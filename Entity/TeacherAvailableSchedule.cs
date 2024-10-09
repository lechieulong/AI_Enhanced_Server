﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class TeacherAvailableSchedule
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Price { get; set; }
        public string Link {  get; set; }
        public string TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public ApplicationUser Teacher { get; set; }
        public bool IsBooked { get; set; }
        public BookedTeacherSession? BookedTeacherSession { get; set; }
    }
}
