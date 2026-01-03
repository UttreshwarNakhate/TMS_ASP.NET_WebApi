using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketManagement.Application.DTOs.Ticket;
using TicketManagement.Application.Interfaces;

namespace TicketManagement.API.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        /// <summary>
        /// Helper method to safely extract userId from claims
        /// </summary>
        private string? GetUserId()
        {
            // Try standard ClaimTypes.NameIdentifier first
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // If not found, try the full URI format
            if (string.IsNullOrEmpty(userId))
            {
                userId = User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            }

            // If still not found, try case-insensitive search
            if (string.IsNullOrEmpty(userId))
            {
                userId = User.Claims
                    .FirstOrDefault(c => c.Type.EndsWith("nameidentifier", StringComparison.OrdinalIgnoreCase))?
                    .Value;
            }

            return userId;
        }

        //Post api to create ticket
        [HttpPost]
        public async Task<IActionResult> CreateTicket(CreateTicketDto dto)
        {
            //Model validation
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            //Get logged-in userId from the jwt token
            var userId = GetUserId();
            Console.WriteLine($"[CreateTicket] UserId extracted: {userId ?? "NULL"}");


            //Safety check(Should not happen normally)
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("[CreateTicket] ❌ UserId is NULL - All claims:");
                foreach (var claim in User.Claims)
                {
                    Console.WriteLine($"  • {claim.Type}: {claim.Value}");
                }
                return Unauthorized(new { error = "UserId claim not found in token" });
            }

            //Call service to create ticket
            var response = await _ticketService.CreateTicketAsync(dto, userId);

            //Return standard response
            return StatusCode(response.StatusCode, response);
        }



        //Get api to fetch all tickets for logged-in user
        [HttpGet("my-tickets")]
        public async Task<IActionResult> GetTickets()
        {
            //Get logged-in userId from the jwt token
            var userId = GetUserId();
            Console.WriteLine($"[GetTickets] UserId extracted: {userId ?? "NULL"}");


            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("[GetTickets] ❌ UserId is NULL - All claims:");
                foreach (var claim in User.Claims)
                {
                    Console.WriteLine($"  • {claim.Type}: {claim.Value}");
                }
                return Unauthorized(new { error = "UserId claim not found in token" });
            }

            //call service to get tickets
            var response = await _ticketService.GetAllTicketsAsync(userId);

            //Return standard response
            return StatusCode(response.StatusCode, response);
        }
    }
}
