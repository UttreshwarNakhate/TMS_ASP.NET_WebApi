using TicketManagement.Domain.Enums;

namespace TicketManagement.Application.DTOs.Ticket
{
    // DTO for showing full ticket details
    public class TicketDetailsDto
    {
        public int Id { get; set; }                  // Ticket Id
        public string Title { get; set; } = "";      // Title
        public string Description { get; set; } = ""; // Full description
        public TicketPriority Priority { get; set; } // Low/Medium/High
        public TicketStatus Status { get; set; }     // Open/InProgress/Closed
        public DateTime CreatedAt { get; set; }      // Created date
        public DateTime? UpdatedAt { get; set; }     // Updated date
    }
}
