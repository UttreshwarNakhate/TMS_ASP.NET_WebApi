using TicketManagement.Application.DTOs.Common;
using TicketManagement.Application.DTOs.Ticket;
using TicketManagement.Domain.Entities;

namespace TicketManagement.Application.Interfaces
{
    public interface ITicketService
    {
        //create ticket for logged-in user
        Task<ApiResponseDto<object>> CreateTicketAsync(CreateTicketDto dto, string userId);

        //get all tickets for logged-in user
        Task<ApiResponseDto<List<GetTicketDto>>> GetAllTicketsAsync(string userId);
    
        // Get single ticket details by Id for logged-in user
        Task<ApiResponseDto<TicketDetailsDto>> GetTicketDetailsByIdAsync(
            int ticketId,
            string userId
        );

        //Update ticket status of logged-in user (Close / Re-open)
        Task<ApiResponseDto<object>> UpdateTicketStatusAsync(int ticketId, string userId, UpdateTicketStatusDto dto);
    }
}
