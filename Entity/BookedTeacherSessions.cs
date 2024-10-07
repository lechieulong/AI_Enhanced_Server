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
        public DateTime BookedDate { get; set; }
        public DateOnly AvailableDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Link { get; set; }
        public string ScheduleId { get; set; }
        [ForeignKey("ScheduleId")]
        public string LearnerId { get; set; }
        [ForeignKey("LearnerId")]
        public ApplicationUser Learner { get; set; }
        public TeacherAvailableSchedule TeacherAvailableSchedule { get; set; }
    }
}
