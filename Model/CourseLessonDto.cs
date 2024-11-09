using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CourseLessonDto
    {
        public Guid CoursePartId { get; set; }

        public string Title { get; set; }
    }
}
