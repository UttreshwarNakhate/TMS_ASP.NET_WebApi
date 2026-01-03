using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TicketManagement.Application.Constants;
using TicketManagement.Application.DTOs.Auth;
using TicketManagement.Application.DTOs.Common;
using TicketManagement.Application.Helpers;
using TicketManagement.Application.Interfaces;
using TicketManagement.Domain.Entities;

namespace TicketManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtTokenHelper _jwtTokenHelper;

        public AuthService(UserManager<ApplicationUser> userManager, JwtTokenHelper jwtTokenHelper)
        {
            _userManager = userManager;
            _jwtTokenHelper = jwtTokenHelper;
        }

        public async Task<ApiResponseDto<object>> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return new ApiResponseDto<object>
                {
                    StatusCode = ApiStatusConstants.BadRequestCode,
                    StatusDesc = ApiStatusConstants.UserAlreadyExists,
                    StatusType = ApiStatusConstants.ErrorType,
                    Details = null
                };
            }

            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return new ApiResponseDto<object>
                {
                    StatusCode = ApiStatusConstants.BadRequestCode,
                    StatusDesc = ApiStatusConstants.RegistrationFailed,
                    StatusType = ApiStatusConstants.ErrorType,
                    Details = result.Errors
                };
            }

            return new ApiResponseDto<object>
            {
                StatusCode = ApiStatusConstants.SuccessCode,
                StatusDesc = ApiStatusConstants.UserCreated,
                StatusType = ApiStatusConstants.SuccessType,
                Details = new { result = result.Succeeded }
            };
        }

        public async Task<ApiResponseDto<object>> LoginAsync(LoginDto loginDto)
        {
            Console.WriteLine($"\n🔐 LOGIN ATTEMPT: {loginDto.Email}");

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                Console.WriteLine($"❌ USER NOT FOUND: {loginDto.Email}");
                return new ApiResponseDto<object>
                {
                    StatusCode = ApiStatusConstants.NotFoundCode,
                    StatusDesc = ApiStatusConstants.WrongEmail,
                    StatusType = ApiStatusConstants.ErrorType,
                    Details = null
                };
            }

            Console.WriteLine($"✓ USER FOUND: ID={user.Id}, Email={user.Email}");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
            {
                Console.WriteLine($"❌ INVALID PASSWORD for {user.Email}");
                return new ApiResponseDto<object>
                {
                    StatusCode = ApiStatusConstants.BadRequestCode,
                    StatusDesc = ApiStatusConstants.WrongPassword,
                    StatusType = ApiStatusConstants.ErrorType,
                    Details = null
                };
            }

            Console.WriteLine("✓ PASSWORD VALIDATED");

            var token = _jwtTokenHelper.GenerateToken(user);

            Console.WriteLine($"✓ TOKEN GENERATED (length: {token?.Length ?? 0})");

            return new ApiResponseDto<object>
            {
                StatusCode = ApiStatusConstants.SuccessCode,
                StatusDesc = ApiStatusConstants.LoginSuccess,
                StatusType = ApiStatusConstants.SuccessType,
                Details = new { user.Id, user.Email, token }
            };
        }
    }
}