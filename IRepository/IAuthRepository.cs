using Model;
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
    }
}
