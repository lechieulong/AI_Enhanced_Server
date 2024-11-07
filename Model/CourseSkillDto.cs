using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Model
{
    public class CourseSkillDto
    {
        public Guid CourseId { get; set; }
        public string Type { get; set; }

        public string Description { get; set; }
    }
}
