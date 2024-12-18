﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class UserDto
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DOB { get; set; }
        public string? ImageURL { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public string? LockReason { get; set; }
        public bool LockoutEnabled { get; set; }
        public UserEducationDto? UserEducationDto { get; set; }
        public List<string>? Roles { get; set; }
    }
}
