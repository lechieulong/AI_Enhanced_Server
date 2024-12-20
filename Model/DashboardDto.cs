using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class DashboardDto
    {
        public int TotalUser { get; set; }
        public int TotalTeachers { get; set; }
        public int Courses { get; set; }
        public int TotalEnrollments { get; set; }

        // Add any other properties that are relevant for your dashboard
    }
}

