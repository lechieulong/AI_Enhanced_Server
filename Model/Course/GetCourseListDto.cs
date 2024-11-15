using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Model.Course
{
    public class GetCourseListDto
    {
        public Guid Id { get; set; }
        public string CourseName { get; set; }
        public string Content { get; set; }
        public int Hours { get; set; }
        public int Days { get; set; }
        public List<string> Categories { get; set; } = new List<string>();
        public double Price { get; set; }
        public string? ImageUrl { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string TeacherName { get; set; }
        public bool IsEnabled { get; set; } = true;
    }
}
