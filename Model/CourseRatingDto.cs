using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CourseRatingDto
    {
        public string UserId { get; set; }
        public int RatingValue { get; set; }
        public string Review { get; set; }
        public DateTime RatedAt { get; set; }
    }

}
