using Entity.Data;
using Google;
using IRepository;
using Microsoft.EntityFrameworkCore;
using Model;
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
        public UserRepository(AppDbContext context)
        {
            _db = context;
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
