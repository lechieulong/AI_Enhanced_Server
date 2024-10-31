using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class UpdateProfileDto
    {
        public string name { get; set; }
        public string phoneNumber { get; set; }
        public DateTime? dob { get; set; }
        public string imageURL { get; set; }
    }
}
