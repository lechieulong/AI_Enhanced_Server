using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Entity.CourseFolder
{
    public class CoursePartDto
    {
        public Guid CourseSkillId { get; set; } 
        public string Title { get; set; }
        public string ContentType { get; set; }

        public string ContentUrl { get; set; }

        public int Order { get; set; }
    }
}
