using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class UserEducation
    {
        [Key, ForeignKey("Teacher")]
        public string TeacherId { get; set; }
        public ApplicationUser Teacher { get; set; }
        public string AboutMe { get; set; }
        public double Grade { get; set; }
        public string DegreeURL { get; set; }
        public string Career {  get; set; }
        public int YearExperience { get; set; }
        public ICollection<Specialization>? Specializations { get; set; }
    }
}
