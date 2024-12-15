﻿using System;

namespace Model
{
    public class ClassDto
    {
        public Guid Id { get; set; }
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        public Guid CourseId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsEnabled { get; set; } = true;
        public int EnrollmentCount { get; set; }
    }
}
