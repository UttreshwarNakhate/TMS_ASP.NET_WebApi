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
        public async Task<ApiResponseDto<object>> CreateTicketAsync(CreateTicketDto dto, string userId)
        {

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

        // Get single ticket details by Id for logged-in user
        public async Task<ApiResponseDto<TicketDetailsDto>> GetTicketDetailsByIdAsync(int ticketId, string userId)
        {
            //Fetch tickets that match both
            //1. Ticket id
            //2. Created by logged-in user
            var ticket = await _dbContext.Tickets
                .Where(t => t.Id == ticketId && t.CreatedByUserId == userId)
                .Select(t => new TicketDetailsDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Priority = t.Priority,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .FirstOrDefaultAsync();

            //If ticket not found return response
            if (ticket == null)
            {
                return new ApiResponseDto<TicketDetailsDto>
                {
                    StatusCode = ApiStatusConstants.NotFoundCode,
                    StatusDesc = ApiStatusConstants.TicketsNotFound,
                    StatusType = ApiStatusConstants.ErrorType,
                    Details = null
                };
            }


            //Return success response
            return new ApiResponseDto<TicketDetailsDto>
            {

                StatusCode = ApiStatusConstants.SuccessCode,
                StatusDesc = ApiStatusConstants.TicketDetailsFetched,
                StatusType = ApiStatusConstants.SuccessType,
                Details = ticket
            };

        }


        public async Task<ApiResponseDto<object>> UpdateTicketStatusAsync(int ticketId, string userId, UpdateTicketStatusDto dto)
        {
            //Fetch tickets owned by this user
            var ticket = await _dbContext.Tickets
                .FirstOrDefaultAsync(t => t.Id == ticketId && t.CreatedByUserId == userId);
            
            //If ticket not found or not owned
            if(ticket == null)
            {
                return new ApiResponseDto<object>
                {
                    StatusCode = ApiStatusConstants.NotFoundCode,
                    StatusDesc = ApiStatusConstants.TicketsNotFound,
                    StatusType = ApiStatusConstants.ErrorType,
                    Details = null
                };
            }

            //Bussiness Rule: If alredy same status, reject it
            if(ticket.Status == dto.Status)
            {
                return new ApiResponseDto<object>
                {
                    StatusCode = ApiStatusConstants.BadRequestCode,
                    StatusDesc = ApiStatusConstants.StatusAlreadyExists,
                    StatusType = ApiStatusConstants.ErrorType,
                    Details = null
                };
            }

            //Update status
            ticket.Status = dto.Status;
            ticket.UpdatedAt = DateTime.UtcNow;

            //Save changes
            await _dbContext.SaveChangesAsync();

            //Return response
            return new ApiResponseDto<object>
            {
                StatusCode = 200,
                StatusDesc = ApiStatusConstants.StatusUpdate,
                StatusType = ApiStatusConstants.SuccessType,
                Details = new
                {
                    ticket.Id,
                    ticket.Status
                }
            };
        }
    }
}
