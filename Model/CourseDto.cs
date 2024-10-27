﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Model
{
    public class CourseDto
    {
        public string UserId { get; set; }
        public string CourseName { get; set; }
        public string Content { get; set; }
        public int Hours { get; set; }
        public int Days { get; set; }
        public List<string> Categories { get; set; } = new List<string>(); // Allow multiple categories
        public double Price { get; set; }

    }
}
