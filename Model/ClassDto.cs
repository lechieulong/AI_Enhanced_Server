using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ClassDto
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        //Number of student
        public int Count { get; set; }
        public int CourseId { get; set; }
        public DateTime StartDate { get; set; }
        public string ImageUrl { get; set; }
    }
}
