﻿namespace Auth.Models.Dto
{
    public class LoginReponseDto
    {
        public UserDto User { get; set; }
        public string Token { get; set; }
    }
}
