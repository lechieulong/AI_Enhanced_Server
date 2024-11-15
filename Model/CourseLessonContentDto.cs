using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CourseLessonContentDto
    {

        public Guid CourseLessonId { get; set; }
        public string ContentType { get; set; }

        [Column(TypeName = "text")]
        public string? ContentText { get; set; }

        public string? ContentUrl { get; set; }

        public int Order { get; set; }
    }
}
