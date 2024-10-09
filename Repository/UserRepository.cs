using Entity;
using Entity.Data;
using Google;
using IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<ApplicationUser>> GetTopTeachersAsync()
        {
            var allUsers = await _db.ApplicationUsers.ToListAsync();

            var teacherUsers = new List<ApplicationUser>();
            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains(SD.Teacher))
                {
                    teacherUsers.Add(user);
                }
            }

            return teacherUsers.Take(10);
        }

        public async Task<UserDto> GetUserProfileByUsernameAsync(string username)
        {
            var user = await _db.ApplicationUsers
                .Where(u => u.UserName.ToLower() == username.ToLower())
                .Select(u => new UserDto
                {
                    UserName = u.UserName,
                    Name = u.Name,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    DOB = u.DOB,
                    ImageURL = u.ImageURL
                })
                .FirstOrDefaultAsync();

            return user;
        }
    }
}
