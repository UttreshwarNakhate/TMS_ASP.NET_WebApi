using TicketManagement.Application.DTOs.Common;
using TicketManagement.Application.DTOs.Ticket;

namespace TicketManagement.Application.Interfaces
{
    public interface ITicketService
    {
        //create ticket for logged-in user
        Task<ApiResponseDto<object>> CreateTicketAsync(CreateTicketDto dto, string userId);

        //get all tickets for logged-in user
        Task<ApiResponseDto<List<GetTicketDto>>> GetAllTicketsAsync(string userId);
    }
}
