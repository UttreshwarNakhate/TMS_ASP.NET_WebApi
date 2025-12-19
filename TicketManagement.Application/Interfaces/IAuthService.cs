using TicketManagement.Application.DTOs.Auth;
using TicketManagement.Application.DTOs.Common;

namespace TicketManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponseDto<Object>> RegisterAsync(RegisterDto registerDto);
    }
}
