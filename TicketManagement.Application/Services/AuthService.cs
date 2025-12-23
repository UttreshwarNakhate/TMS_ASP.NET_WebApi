

using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
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

        //create constructor
        public AuthService(UserManager<ApplicationUser> userManager,  JwtTokenHelper jwtTokenHelper)
        {
            _userManager = userManager;
            _jwtTokenHelper = jwtTokenHelper;
        }


        //Following method is used to register the user
        public async Task<ApiResponseDto<object>> RegisterAsync(RegisterDto registerDto)
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
   
    
        //Following method is used to login user
        public async Task<ApiResponseDto<object>> LoginAsync(LoginDto loginDto)
        {
            //Find user by email id
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            //if user not exists
            if (user == null)
            {
                return new ApiResponseDto<object>
                {
                    StatusCode = ApiStatusConstants.NotFoundCode,
                    StatusDesc = ApiStatusConstants.WrongEmail,
                    StatusType = ApiStatusConstants.ErrorType,
                    Details = null
                };
            }

            //Validate password
            var isPassWordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPassWordValid)
            {
                return new ApiResponseDto<object>
                {
                    StatusCode = ApiStatusConstants.BadRequestCode,
                    StatusDesc = ApiStatusConstants.WrongPassword,
                    StatusType = ApiStatusConstants.ErrorType,
                    Details = null
                };
            }

            //Generate  token
            var token = _jwtTokenHelper.GenerateToken(user);


            //return success response
            return new ApiResponseDto<object>
            {
                StatusCode = ApiStatusConstants.SuccessCode,
                StatusDesc = ApiStatusConstants.LoginSuccess,
                StatusType = ApiStatusConstants.SuccessType,
                Details = user
            };

        }
    }
}
