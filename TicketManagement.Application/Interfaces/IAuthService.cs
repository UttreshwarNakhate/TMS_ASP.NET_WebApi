using TicketManagement.Application.DTOs.Auth;
using TicketManagement.Application.DTOs.Common;

namespace TicketManagement.Application.Interfaces
{
    public interface IAuthService
    {
        //Register method contract
        Task<ApiResponseDto<Object>> RegisterAsync(RegisterDto registerDto);

        //Login method contract
        Task<ApiResponseDto<Object>> LoginAsync(LoginDto loginDto);
    }
}
