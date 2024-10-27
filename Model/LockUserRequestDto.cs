using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class LockUserRequestDto
    {
        public string UserId { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
