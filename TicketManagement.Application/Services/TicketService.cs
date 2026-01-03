using TicketManagement.Application.DTOs.Ticket;
using TicketManagement.Application.Interfaces;

using TicketManagement.Application.Constants;
using TicketManagement.Domain.Entities;
using TicketManagement.Domain.Enums;
using TicketManagement.Infrastructure.Data;
using TicketManagement.Application.DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace TicketManagement.Application.Services
{
    public class TicketService : ITicketService
    {   
        private readonly AppDbContext _dbContext;

        // Inject DbContext (same like injecting prisma / sequelize)
        public TicketService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Create Ticket for logged-in user
        public async Task<ApiResponseDto<object>> CreateTicketAsync(CreateTicketDto dto, string userId) {

            //Create Ticket Entity
            var ticket = new Ticket
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                Status = TicketStatus.Open,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };


            // Add ticket to DbContext
            _dbContext.Tickets.Add(ticket);


            //Save to database
            await _dbContext.SaveChangesAsync();

            //Return success response
            return new ApiResponseDto<object>
            {
                StatusCode = ApiStatusConstants.CreatedCode,
                StatusDesc = ApiStatusConstants.TicketCreated,
                StatusType = ApiStatusConstants.SuccessType,
                Details = new
                {
                    ticket.Id,
                    ticket.Title,
                    ticket.Status
                }
            };


        }


        //Get all tickets for logged-in user
        public async Task<ApiResponseDto<List<GetTicketDto>>> GetAllTicketsAsync(string userId)
        {
            //Fetch tickets created by the user
            var tickets = await _dbContext.Tickets
                .Where(t => t.CreatedByUserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new GetTicketDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Priority = t.Priority,
                    Status = t.Status,
                    CreatedByUserId = userId,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                }).ToListAsync();


            //Return response
            return new ApiResponseDto<List<GetTicketDto>>
            {
                StatusCode = ApiStatusConstants.SuccessCode,
                StatusDesc = ApiStatusConstants.TicketsFetched,
                StatusType = ApiStatusConstants.SuccessType,
                Details = tickets
            };
        }
   
    
    }
}
