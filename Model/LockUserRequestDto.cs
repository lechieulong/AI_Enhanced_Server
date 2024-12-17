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
        public string LockoutReason { get; set; }
        public bool LockoutForever { get; set; }
        public int DurationValue { get; set; } // Optional: Duration value for custom locks
        public string DurationUnit { get; set; }
    }
}
