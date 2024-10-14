using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class UserEducationDto
    {
        public string TeacherId { get; set; }
        public string AboutMe { get; set; }
        public double Grade { get; set; }
        public string DegreeURL { get; set; }
        public string Career { get; set; }
        public int YearExperience { get; set; }
        public string? Specialization { get; set; }
        public bool IsApprove { get; set; } = false;
    }
}
