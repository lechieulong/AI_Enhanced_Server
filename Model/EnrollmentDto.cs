using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class EnrollmentDto
    {
        public Guid CourseId { get; set; }
        public string UserId { get; set; }

        public Guid ClassId { get; set; }
        public string EnrollAt { get; set; }

        //public string ClassId { get; set; }
    }

}
