﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class LoginReponseDto
    {
        public UserDto User { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }
}
