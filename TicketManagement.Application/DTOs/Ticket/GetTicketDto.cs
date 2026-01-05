using TicketManagement.Domain.Enums;

namespace TicketManagement.Application.DTOs.Ticket
{
    public class GetTicketDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; }
        public TicketPriority Priority { get; set; }
        public TicketStatus Status { get; set; }
        public string CreatedByUserId { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
