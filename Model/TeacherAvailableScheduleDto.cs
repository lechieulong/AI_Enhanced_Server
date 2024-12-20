﻿using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class TeacherAvailableScheduleDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime StartTime { get; set; }
        public int Minutes { get; set; }
        // 0 is availabe, 1 is pending, 2 is booked
        public ScheduleStatus Status { get; set; }
        public Int64 Price { get; set; }
        public string Link { get; set; }
        public string TeacherId { get; set; }
        public DateTime? PendingTime { get; set; }
        [NotMapped]
        public DateTime EndTime => StartTime.AddMinutes(Minutes);
    }
}