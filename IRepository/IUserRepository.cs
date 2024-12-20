﻿using Entity;
using Microsoft.AspNetCore.Http;
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
        Task<IEnumerable<UserDto>> GetTopTeachersAsync(string userId);
        Task<IEnumerable<TeacherSearchDto>> SearchTeachersAsync(string searchText);
        Task<UserDto> GetUserProfileByUsernameAsync(string username);
        Task<UserDto> GetUserUserByIdAsync(string userId);
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<(IEnumerable<UserDto> users, int totalCount)> GetUsersAsync(int page, int pageSize);
        Task LockUserAsync(string userId, int? durationValue, string durationUnit, bool lockoutForever, string lockoutReason);
        Task UnlockUserAsync(string userId);
        Task<IEnumerable<UserFromFileDto>> ImportUserAsync(IFormFile file);
        Task<ApplicationUser> GetUserEducationByUSerName(string username);
        Task<string> UpdateRole(string username);
        Task<DashboardDto> Analysis();
    }
}
