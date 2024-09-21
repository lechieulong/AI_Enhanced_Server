using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Email
{
    public class RemindMemberRequestDto
    {
        //public string RecipientName { get; set; }
        public string RecipientEmail { get; set; }
        public string ReminderMessage { get; set; }
    }


}
