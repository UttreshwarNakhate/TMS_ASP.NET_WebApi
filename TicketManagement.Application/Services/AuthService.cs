using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TicketManagement.Application.Constants;
using TicketManagement.Application.DTOs.Auth;
using TicketManagement.Application.DTOs.Common;
using TicketManagement.Application.Helpers;
using TicketManagement.Application.Interfaces;
using TicketManagement.Domain.Entities;
using TicketManagement.Infrastructure.Data;

namespace TicketManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtTokenHelper _jwtTokenHelper;
        private readonly AppDbContext _dbContext;
        public AuthService(UserManager<ApplicationUser> userManager, JwtTokenHelper jwtTokenHelper, AppDbContext dbContext)
        {
            _userManager = userManager;
            _jwtTokenHelper = jwtTokenHelper;
            _dbContext = dbContext;
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
            // 1️⃣ Find user by email (same as Node.js: User.findOne)
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
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

            // 2️⃣ Validate password (bcrypt.compare equivalent)
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
            {
                return new ApiResponseDto<object>
                {
                    StatusCode = ApiStatusConstants.BadRequestCode,
                    StatusDesc = ApiStatusConstants.WrongPassword,
                    StatusType = ApiStatusConstants.ErrorType,
                    Details = null
                };
            }

            // 3️⃣ Generate ACCESS TOKEN (short lived)
            var accessToken = _jwtTokenHelper.GenerateAccessToken(user);

            // 4️⃣ Generate REFRESH TOKEN (long lived, random)
            var refreshToken = _jwtTokenHelper.GenerateRefreshToken();

            // 5️⃣ Store refresh token in DB (same as Node.js design)
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // long lifespan
                IsRevoked = false
            };

            _dbContext.RefreshTokens.Add(refreshTokenEntity);
            await _dbContext.SaveChangesAsync();

            // 6️⃣ Return both tokens to client
            return new ApiResponseDto<object>
            {
                StatusCode = ApiStatusConstants.SuccessCode,
                StatusDesc = ApiStatusConstants.LoginSuccess,
                StatusType = ApiStatusConstants.SuccessType,
                Details = new
                {
                    user.Id,
                    user.Email,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            };
        }


        public async Task<ApiResponseDto<object>> LogoutAsync(string userId)
        {
            // NOTE:
            // With JWT (stateless), logout is handled on client side
            // Server just confirms logout

            // We keep method async for future refresh-token logic
            await Task.CompletedTask;

            return new ApiResponseDto<object>
            {
                StatusCode = 200,
                StatusDesc = "Logout successful",
                StatusType = ApiStatusConstants.SuccessType,
                Details = new
                {
                    UserId = userId
                }
            };
        }

        //Refresh token service
        public async Task<ApiResponseDto<object>> RefreshTokenAsync(RefreshTokenDto dto)
        {
            // Find refresh token in DB
            var storedToken = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt =>
                    rt.Token == dto.RefreshToken &&
                    !rt.IsRevoked &&
                    rt.ExpiresAt > DateTime.UtcNow);

            if (storedToken == null)
            {
                return new ApiResponseDto<object>
                {
                    StatusCode = 401,
                    StatusDesc = "Invalid or expired refresh token",
                    StatusType = "E",
                    Details = null
                };
            }

            // Get user
            var user = await _userManager.FindByIdAsync(storedToken.UserId);

            // Rotate refresh token (BEST PRACTICE)
            storedToken.IsRevoked = true;

            var newRefreshToken = _jwtTokenHelper.GenerateRefreshToken();

            _dbContext.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                UserId = user!.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            });

            var newAccessToken = _jwtTokenHelper.GenerateAccessToken(user);

            await _dbContext.SaveChangesAsync();

            return new ApiResponseDto<object>
            {
                StatusCode = 200,
                StatusDesc = "Token refreshed successfully",
                StatusType = "S",
                Details = new
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                }
            };
        }

    }
}