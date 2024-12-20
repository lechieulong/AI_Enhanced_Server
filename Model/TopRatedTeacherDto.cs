using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class TopRatedTeacherDto
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string? ImageURL { get; set; }
        public double AverageRating { get; set; }
        public int RatingCount { get; set; }
    }
}
