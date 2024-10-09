using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class BookedTeacherSession
    {
        public Guid Id { get; set; }
        public Guid ScheduleId { get; set; }
        [ForeignKey("ScheduleId")]
        public TeacherAvailableSchedule TeacherAvailableSchedule { get; set; }
        public string LearnerId { get; set; }
        [ForeignKey("LearnerId")]
        public ApplicationUser Learner { get; set; }
        public DateTime BookedDate { get; set; }
    }
}
