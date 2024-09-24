using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Class
    {
        [Key]
        public int Id { get; set; }
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        //Number of student
        public int Count { get; set; }
        public int CourseId { get; set; }
        public DateTime StartDate { get; set; }
        [NotMapped]
        public string ImageUrl { get; set; }
    }
}

