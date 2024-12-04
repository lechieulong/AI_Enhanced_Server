using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class GetSessionsByUserIdDto
    {
        public Guid Id { get; set; }
        public Guid ScheduleId { get; set; }
        public GetSessionsTeacherAvailableDto TeacherAvailableSchedule { get; set; }
        public string LearnerId { get; set; }
        public UserDto Learner { get; set; }
        public DateTime BookedDate { get; set; }
    }
}
