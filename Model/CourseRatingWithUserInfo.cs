using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CourseRatingWithUserInfo
    {
        public int RatingValue { get; set; }
        public string Review { get; set; }
        public string RatedAt { get; set; }  
        public string Username { get; set; }
        public string ImageUrl { get; set; }
    }


}
