﻿using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository
{
    public interface IAuthRepository
    {
        Task<string> Register(RegistrationRequestDto registrationRequestModel);
        Task<LoginReponseDto> Login(LoginRequestDto loginRequestModel);
        Task<bool> AssignRole(string email, string roleName);
        Task<bool> CheckEmailExists(string email);
        Task<string> RegisterWithGoogle(string email, string token);
        Task<LoginReponseDto> LoginWithGoogle(string token);
    }
}
