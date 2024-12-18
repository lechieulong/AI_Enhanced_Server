using Entity;
using Entity.Data;
using Google;
using IRepository;
using IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Utility;
using OfficeOpenXml;
using StackExchange.Redis;
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
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IEmailSenderService _emailSenderService;
        public UserRepository(AppDbContext context, UserManager<ApplicationUser> userManager, IEmailSenderService emailSender, IJwtTokenGenerator jwtTokenGenerator)
        {
            _db = context;
            _userManager = userManager;
            _emailSenderService = emailSender;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<IEnumerable<UserDto>> GetTopTeachersAsync(string currentUserId)
        {
            //var allUsers = await _db.ApplicationUsers.ToListAsync();

            //var teacherUsers = new List<ApplicationUser>();
            //foreach (var user in allUsers)
            //{
            //    var roles = await _userManager.GetRolesAsync(user);
            //    if (roles.Contains(SD.Teacher) && user.Id != currentUserId && !roles.Contains(SD.Admin))
            //    {
            //        teacherUsers.Add(user);
            //    }
            //}
            var teacherUsers = await (from user in _db.ApplicationUsers
                                      join userRole in _db.UserRoles on user.Id equals userRole.UserId
                                      join role in _db.Roles on userRole.RoleId equals role.Id
                                      where role.Name == SD.Teacher &&
                                            user.Id != currentUserId &&
                                            !(from ur in _db.UserRoles
                                              join r in _db.Roles on ur.RoleId equals r.Id
                                              where ur.UserId == user.Id && r.Name == SD.Admin
                                              select ur).Any()
                                      select new UserDto
                                      {
                                          ID = user.Id,
                                          UserName = user.UserName,
                                          Name = user.Name,
                                          Email = user.Email,
                                          PhoneNumber = user.PhoneNumber,
                                          ImageURL = user.ImageURL
                                      })
              .Take(10)
              .AsNoTracking()
              .ToListAsync();

            return teacherUsers;
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
                    ID = u.Id,
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

        public async Task<UserDto> GetUserUserByIdAsync(string userId)
        {
            var user = await _db.ApplicationUsers
                .Where(u => u.Id.ToLower() == userId.ToLower())
                .Select(u => new UserDto
                {
                    ID = u.Id,
                    UserName = u.UserName,
                    Name = u.Name,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    DOB = u.DOB,
                    ImageURL = u.ImageURL,
                })
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _db.ApplicationUsers.FindAsync(userId);
        }
        public async Task<(IEnumerable<UserDto> users, int totalCount)> GetUsersAsync(int page, int pageSize)
        {
            var query = from user in _db.ApplicationUsers
                        join userRole in _db.UserRoles on user.Id equals userRole.UserId
                        join role in _db.Roles on userRole.RoleId equals role.Id
                        where !(from ur in _db.UserRoles
                                join r in _db.Roles on ur.RoleId equals r.Id
                                where ur.UserId == user.Id && r.Name == SD.Admin
                                select ur).Any()
                        select user;

            var totalCount = await query.Distinct().CountAsync();

            var users = await query
                .Distinct()
                .OrderBy(u => u.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                var userDto = new UserDto
                {
                    ID = user.Id,
                    UserName = user.UserName,
                    Name = user.Name,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    DOB = user.DOB,
                    ImageURL = user.ImageURL,
                    LockoutEnd = user.LockoutEnd,
                    LockReason = user.LockoutReason,
                    LockoutEnabled = user.LockoutEnabled,
                    Roles = roles.ToList()
                };

                userDtos.Add(userDto);
            }

            return (userDtos, totalCount);
        }

        public async Task LockUserAsync(string userId, int? durationValue, string durationUnit, bool lockoutForever, string lockoutReason)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                if (lockoutForever)
                {
                    // Lock the user indefinitely
                    user.LockoutEnd = DateTimeOffset.MaxValue;
                    user.LockoutForever = true;
                    user.LockoutReason = lockoutReason;
                }
                else
                {
                    // Calculate lockout duration
                    var lockoutDuration = CalculateLockoutDuration(durationValue, durationUnit);
                    if (lockoutDuration.HasValue)
                    {
                        user.LockoutEnd = DateTime.UtcNow.Add(lockoutDuration.Value);
                        user.LockoutForever = false;
                        user.LockoutReason = lockoutReason;
                    }
                    else
                    {
                        throw new ArgumentException("Invalid lockout duration or unit.");
                    }
                }

                await _userManager.UpdateAsync(user);
            }
            else
            {
                throw new InvalidOperationException("User not found.");
            }
        }

        private TimeSpan? CalculateLockoutDuration(int? durationValue, string durationUnit)
        {
            if (durationValue == null || durationValue <= 0 || string.IsNullOrWhiteSpace(durationUnit))
            {
                return null;
            }

            return durationUnit.ToLower() switch
            {
                "minute" => TimeSpan.FromMinutes(durationValue.Value),
                "hour" => TimeSpan.FromHours(durationValue.Value),
                "day" => TimeSpan.FromDays(durationValue.Value),
                "week" => TimeSpan.FromDays(durationValue.Value * 7),  // 1 tuần = 7 ngày
                "month" => TimeSpan.FromDays(durationValue.Value * 30), // 1 tháng = 30 ngày (ước tính)
                "year" => TimeSpan.FromDays(durationValue.Value * 365), // 1 nam = 365 ngày (ước tính)
                _ => null // Invalid unit
            };
        }

        public async Task UnlockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.LockoutEnd = DateTime.Now;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    var unlockDate = DateTime.Now;
                    var email = user.Email; // Assuming user has an Email property
                    var name = user.Name; // Assuming user has a UserName property

                    await _emailSenderService.SendEmailUnlockUser(email, name, unlockDate);
                }
            }
        }

        public async Task<IEnumerable<UserFromFileDto>> ImportUserAsync(IFormFile file)
        {
            var users = new List<UserFromFileDto>();

            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or not provided.");

            // Read file contents
            using (var package = new ExcelPackage(file.OpenReadStream()))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Lay sheet dau tien
                var rowCount = worksheet.Dimension?.Rows ?? 0; //Dem so hang trong sheet

                for (int row = 2; row <= rowCount; row++) // Bo qua header, lay row 2
                {
                    var user = new UserFromFileDto
                    {
                        UserName = worksheet.Cells[row, 1].Text.Trim(),
                        Name = worksheet.Cells[row, 2].Text.Trim(),
                        Email = worksheet.Cells[row, 3].Text.Trim(),
                        PhoneNumber = worksheet.Cells[row, 4].Text.Trim(),
                        Password = worksheet.Cells[row, 5].Text.Trim(),
                        DOB = (DateTime)(DateTime.TryParse(worksheet.Cells[row, 6].Text, out var dob) ? dob : (DateTime?)null),
                        ImageURL = worksheet.Cells[row, 7].Text.Trim()
                    };

                    if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
                    {
                        continue; // Skip this row and move to the next one
                    }

                    users.Add(user);
                }
            }

            return users;
        }

        public async Task<ApplicationUser> GetUserEducationByUSerName(string username)
        {
            var userEducation = await _db.ApplicationUsers
                .Include(ue => ue.UserEducation)
                .ThenInclude(ue => ue.Specializations)
                .FirstOrDefaultAsync(r => r.UserName == username);

            if (userEducation == null)
            {
                throw new KeyNotFoundException("No user education found for the specified user ID.");
            }

            return userEducation;
        }

        public async Task<string> UpdateRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);

            var token = _jwtTokenGenerator.GenerateToken(user, roles);
            return token;
        }
    }
}
