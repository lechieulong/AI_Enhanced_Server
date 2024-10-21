using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity
{
    public class Class
    {
        [Key]
        public Guid Id { get; set; }

        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        public int Count { get; set; }

        public Guid CourseId { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; }

        public DateTime StartDate { get; set; }
        public string? ImageUrl { get; set; }

        // Quan hệ nhiều-nhiều với User thông qua UserClass
        public ICollection<UserClass>? UserClasses { get; set; }
    }
}
