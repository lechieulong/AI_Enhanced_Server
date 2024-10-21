using Entity;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository
{
    public interface IUserRepository
    {
        Task<IEnumerable<ApplicationUser>> GetTopTeachersAsync();
        Task<IEnumerable<TeacherSearchDto>> SearchTeachersAsync(string searchText);
        Task<UserDto> GetUserProfileByUsernameAsync(string username);
    }
}
