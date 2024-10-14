﻿using Entity;
using Entity.Data;
using IRepository;
using Microsoft.AspNetCore.Identity;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Utility;
using Microsoft.IdentityModel.Tokens;
using IService;
using Microsoft.EntityFrameworkCore;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IEmailSenderService _emailSenderService; // Thêm dịch vụ
        private readonly IConfiguration _configuration;
        public AuthRepository(AppDbContext appDbContext, IJwtTokenGenerator jwtTokenGenerator,
                          UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
                          IEmailSenderService emailSenderService, IConfiguration configuration)
        {
            _db = appDbContext;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSenderService = emailSenderService;  // Khởi tạo dịch vụ gửi email
            _configuration = configuration;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }
        
        public async Task<LoginReponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = await _userManager.FindByNameAsync(loginRequestDto.Username);

            if (user == null)
            {
                return new LoginReponseDto() { User = null, Token = "", Message = "Invalid username or password." };
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                var lockoutEndDate = await _userManager.GetLockoutEndDateAsync(user);
                // Nếu thời gian kết thúc khóa không phải null, thêm vào thời gian khóa vào thông báo
                if (lockoutEndDate.HasValue)
                {
                    var remainingLockoutTime = lockoutEndDate.Value.UtcDateTime - DateTime.UtcNow;
                    var remainingMinutes = (int)remainingLockoutTime.TotalMinutes;

                    return new LoginReponseDto()
                    {
                        User = null, Token = "", Message = $"Your account is locked due to too many failed login attempts. Please try again in {remainingMinutes} minutes."
                    };
                }
                else
                {
                    // Nếu không có thời gian khóa cụ thể
                    return new LoginReponseDto()
                    {
                        User = null, Token = "", Message = "Your account is locked due to too many failed login attempts. Please try again later."
                    };
                }
            }

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            if (!isValid)
            {
                await _userManager.AccessFailedAsync(user);

                if (await _userManager.IsLockedOutAsync(user))
                {
                    return new LoginReponseDto()
                    {
                        User = null, Token = "", Message = "Your account is locked due to too many failed login attempts. Please try again later."
                    };
                }

                var failedAttempts = await _userManager.GetAccessFailedCountAsync(user);
                if (failedAttempts >= 3)
                {
                    return new LoginReponseDto()
                    {
                        User = null, Token = "", Message = $"Invalid username or password. You have {failedAttempts} failed login attempts."
                    };
                }

                return new LoginReponseDto() { User = null, Token = "", Message = "Invalid username or password." };
            }

            await _userManager.ResetAccessFailedCountAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            // Generate JWT token
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            UserDto userDto = new()
            {
                ID = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                ImageURL = user.ImageURL
            };

            LoginReponseDto loginReponseDto = new LoginReponseDto()
            {
                User = userDto,
                Token = token,
                Message = "Login successful."
            };

            return loginReponseDto;
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            //Add check email exist
            var existingUser = await _userManager.FindByEmailAsync(registrationRequestDto.Email);
            if (existingUser != null)
            {
                return "Email is already taken.";
            }

            ApplicationUser user = new()
            {
                UserName = GetUsernameFromEmail(registrationRequestDto.Email),
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToLower(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber
            };
            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    await AssignRole(user.Email, SD.User);

                    var userToReturn = _db.ApplicationUsers.First(u => u.Email == registrationRequestDto.Email);

                    UserDto userDto = new()
                    {
                        ID = userToReturn.Id,
                        Email = userToReturn.Email,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber
                    };

                    await _emailSenderService.SendRegistrationSuccessEmail(user.Email, user.Name, user.UserName);
                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault()?.Description ?? "Registration failed";
                }
            }
            catch (Exception ex)
            {
                
            }
            return "Error Encountered";
        }

        public string GetUsernameFromEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            {
                throw new ArgumentException("Invalid email format.");
            }

            var username = email.Split('@')[0];
            return username;
        }

        public async Task<bool> CheckEmailExists(string email)
        {
            return await _db.ApplicationUsers.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }
        public async Task<string> RegisterWithGoogle(string email, string token)
        {
            if (await CheckEmailExists(email))
            {
                return "Email already exists.";
            }

            var payload = await GoogleJsonWebSignature.ValidateAsync(token);

            string generatedPassword = GenerateRandomPassword();

            ApplicationUser user = new()
            {
                UserName = GetUsernameFromEmail(email),
                Email = email,
                NormalizedEmail = email.ToLower(),
                Name = payload.Name,
                ImageURL = payload.Picture
            };

            var result = await _userManager.CreateAsync(user, generatedPassword);
            if (result.Succeeded)
            {
                await AssignRole(user.Email, SD.User);
                await _emailSenderService.SendRegistrationGGSuccessEmail(user.Email, user.Name, user.UserName, generatedPassword);
                return "";
            }
            return result.Errors.FirstOrDefault()?.Description ?? "Registration failed";
        }

        public static string GenerateRandomPassword()
        {
            const int passwordLength = 8;
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string specials = "!@#$%^&*()_-+=<>?";

            // Ensure the password has at least one character from each category
            StringBuilder password = new StringBuilder();
            Random random = new Random();

            // Add one character from each required category
            password.Append(upper[random.Next(upper.Length)]); // At least one uppercase
            password.Append(lower[random.Next(lower.Length)]); // At least one lowercase
            password.Append(digits[random.Next(digits.Length)]); // At least one digit
            password.Append(specials[random.Next(specials.Length)]); // At least one special character

            // Fill the rest of the password length with random characters
            string allCharacters = lower + upper + digits + specials;
            for (int i = password.Length; i < passwordLength; i++)
            {
                password.Append(allCharacters[random.Next(allCharacters.Length)]);
            }

            // Shuffle the characters to prevent predictable patterns
            char[] passwordArray = password.ToString().ToCharArray();
            Shuffle(passwordArray);

            return new string(passwordArray);
        }

        // Method to shuffle the characters in the array
        private static void Shuffle(char[] array)
        {
            Random random = new Random();
            int n = array.Length;
            while (n > 1)
            {
                int k = random.Next(n--);
                char temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        public async Task<LoginReponseDto> LoginWithGoogle(string token)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(token);

                string email = payload.Email;

                var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
                if (user == null)
                {
                    await RegisterWithGoogle(email, token);
                    user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
                }

                if (await _userManager.IsLockedOutAsync(user))
                {
                    var lockoutEndDate = await _userManager.GetLockoutEndDateAsync(user);
                    // Nếu thời gian kết thúc khóa không phải null, thêm vào thời gian khóa vào thông báo
                    if (lockoutEndDate.HasValue)
                    {
                        var remainingLockoutTime = lockoutEndDate.Value.UtcDateTime - DateTime.UtcNow;
                        var remainingMinutes = (int)remainingLockoutTime.TotalMinutes;

                        return new LoginReponseDto()
                        {
                            User = null,
                            Token = "",
                            Message = $"Your account is locked due to too many failed login attempts. Please try again in {remainingMinutes} minutes."
                        };
                    }
                    else
                    {
                        // Nếu không có thời gian khóa cụ thể
                        return new LoginReponseDto()
                        {
                            User = null,
                            Token = "",
                            Message = "Your account is locked due to too many failed login attempts. Please try again later."
                        };
                    }
                }

                // Generate JWT token for authenticated user
                var roles = await _userManager.GetRolesAsync(user);
                var jwtToken = _jwtTokenGenerator.GenerateToken(user, roles);

                UserDto userDto = new()
                {
                    ID = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                    ImageURL = user.ImageURL
                };

                return new LoginReponseDto()
                {
                    User = userDto,
                    Token = jwtToken
                };
            }
            catch (InvalidJwtException)
            {
                // Handle token validation failure
                return null;
            }
        }
        public async Task<string> ChangePassword(string email, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return "User not found.";
            }

            var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, currentPassword);
            if (!isCurrentPasswordValid)
            {
                return "Current password is incorrect.";
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                return "Password changed successfully.";
            }
            return result.Errors.FirstOrDefault()?.Description ?? "Password change failed.";
        }
        public async Task<string> RequestPasswordReset(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return "Email does not exist.";
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var baseUrl = _configuration["AllowedOrigins:FrontendUrl"];
            var resetLink = $"{baseUrl}/reset-password?token={encodedToken}&email={email}";

            await _emailSenderService.SendResetPasswordRequestEmail(user.Email, user.Name, resetLink);

            return "";
        }
        public async Task<string> ResetPassword(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return "User not found.";
            }

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            // Check if the token is invalid
            if (result.Errors.Any(e => e.Code == "InvalidToken"))
            {
                return "Please request a new password reset.";
            }

            if (result.Succeeded)
            {
                await _emailSenderService.SendResetPasswordConfirmationEmail(user.Email, user.Name);
                return "Password reset successfully.";
            }
            else
            {
                return result.Errors.FirstOrDefault()?.Description ?? "Password reset failed.";
            }
        }
    }
}
