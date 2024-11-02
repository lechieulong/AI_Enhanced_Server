using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ProcessTeacherRequestDto
    {
        public string Comment { get; set; }
        public RequestStatusEnum Status { get; set; }
    }
}
