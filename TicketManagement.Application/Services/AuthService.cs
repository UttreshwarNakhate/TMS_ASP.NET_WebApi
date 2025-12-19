

using Microsoft.AspNetCore.Identity;
using TicketManagement.Application.Constants;
using TicketManagement.Application.DTOs.Auth;
using TicketManagement.Application.DTOs.Common;
using TicketManagement.Application.Interfaces;
using TicketManagement.Domain.Entities;

namespace TicketManagement.Application.Services
{
      public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        //create constructor
        public AuthService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


        //Following method is used to register the user
        public async Task<ApiResponseDto<Object>> RegisterAsync(RegisterDto registerDto)
        {

            //1. Check if user is already exists
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null) {
                return new ApiResponseDto<Object>
                {
                    StatusCode = ApiStatusConstants.BadRequestCode,
                    StatusDesc = ApiStatusConstants.UserAlreadyExists,
                    StatusType = ApiStatusConstants.ErrorType,
                    Details = null
                };
            }


            var user = new ApplicationUser
            {
                // Using Email as UserName for login
                UserName = registerDto.Email,
                Email = registerDto.Email   
            };


            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if(!result.Succeeded)
            {
                var errorMessage = string.Join(",", result.Errors.Select(e => e.Description));
                // 2️⃣ Handle Identity errors
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ",
                        result.Errors.Select(e => e.Description));

                    return new ApiResponseDto<Object>
                    {
                        StatusCode = ApiStatusConstants.BadRequestCode,
                        StatusDesc = ApiStatusConstants.RegistrationFailed,
                        StatusType = ApiStatusConstants.ErrorType,
                        Details = result.Errors
                    };
                }
            }

            // 4. Success
            return new ApiResponseDto<Object>
            {
                StatusCode = ApiStatusConstants.SuccessCode,
                StatusDesc = ApiStatusConstants.UserCreated,
                StatusType = ApiStatusConstants.SuccessType,
                Details = new { user.Email }
            };
        }
    }
}
