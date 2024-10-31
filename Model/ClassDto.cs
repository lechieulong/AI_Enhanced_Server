﻿using System;

namespace Model
{
    public class ClassDto
    {
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        public Guid CourseId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsEnabled { get; set; } = true; // Mặc định là true
    }
}
