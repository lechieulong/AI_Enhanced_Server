using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class BookedTeacherSessionDto
    {
        public Guid ScheduleId { get; set; }
        public string LearnerId { get; set; }
    }
}
