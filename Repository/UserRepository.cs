﻿using Entity;
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

        public async Task<IEnumerable<TeacherSearchDto>> SearchTeachersAsync(string searchText)
        {
            var allUsers = await _db.ApplicationUsers.ToListAsync();

            var teacherUsers = new List<TeacherSearchDto>();
            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains(SD.Teacher) &&
                    (user.UserName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                     user.Email.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                     user.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)))
                {
                    teacherUsers.Add(new TeacherSearchDto
                    {
                        Name = user.Name,
                        UserName = user.UserName,
                        ImageURL = user.ImageURL
                    });
                }
            }

            return teacherUsers.Take(5);
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
        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            // Tìm kiếm người dùng theo UserId
            return await _db.ApplicationUsers.FindAsync(userId);
        }
        public async Task<(IEnumerable<UserDto> users, int totalCount)> GetUsersAsync(int page, int pageSize)
        {
            var totalCount = await _db.ApplicationUsers.CountAsync();

            var users = await _db.ApplicationUsers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserDto
                {
                    UserName = u.UserName,
                    Name = u.Name,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    DOB = u.DOB,
                    ImageURL = u.ImageURL,
                    LockoutEnd = u.LockoutEnd,
                    LockoutEnabled = u.LockoutEnabled
                })
                .ToListAsync();

            return (users, totalCount);
        }

    }
}
