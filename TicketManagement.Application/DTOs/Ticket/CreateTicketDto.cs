using System.ComponentModel.DataAnnotations;
using TicketManagement.Domain.Enums;

namespace TicketManagement.Application.DTOs.Ticket
{
    // DTO = data coming from frontend (like req.body in Node.js)
    public class CreateTicketDto
    {
        // Ticket title is required
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = null!;

        // Description is required
        [Required]
        public string Description { get; set; } = null!;

        // Priority comes from enum (Low, Medium, High)
        [Required]
        public TicketPriority Priority { get; set; }
    }
}
