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
        private readonly IEmailSenderService _emailSenderService;
        public UserRepository(AppDbContext context, UserManager<ApplicationUser> userManager, IEmailSenderService emailSender)
        {
            _db = context;
            _userManager = userManager;
            _emailSenderService = emailSender;
        }

        public async Task<IEnumerable<ApplicationUser>> GetTopTeachersAsync(string currentUserId)
        {
            var allUsers = await _db.ApplicationUsers.ToListAsync();

            var teacherUsers = new List<ApplicationUser>();
            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains(SD.Teacher) && user.Id != currentUserId && !roles.Contains(SD.Admin))
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
            var allUsers = await _db.ApplicationUsers.ToListAsync();

            var nonAdminUsers = new List<ApplicationUser>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains(SD.Admin))
                {
                    nonAdminUsers.Add(user);
                }
            }

            var totalCount = nonAdminUsers.Count;

            var users = nonAdminUsers
                .OrderBy(u => u.Name) // Sort by Name in alphabetical order
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserDto
                {
                    ID = u.Id,
                    UserName = u.UserName,
                    Name = u.Name,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    DOB = u.DOB,
                    ImageURL = u.ImageURL,
                    LockoutEnd = u.LockoutEnd,
                    LockoutEnabled = u.LockoutEnabled
                })
                .ToList();

            return (users, totalCount);
        }

        public async Task LockUserAsync(string userId, int durationInMinutes)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                //Lock user in durationInMinutes
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(durationInMinutes);
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task UnlockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.LockoutEnd = DateTime.UtcNow;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Convert the current UTC time to Vietnam time (UTC+7)
                    var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                    var unlockDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
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

    }
}
