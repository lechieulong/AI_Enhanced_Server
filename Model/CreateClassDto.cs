using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CreateClassDto
    {
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        public Guid CourseId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsEnabled { get; set; } = true;
        public string? ImageUrl { get; set; }
        public int EnrollmentCount { get; set; }
    }
}
