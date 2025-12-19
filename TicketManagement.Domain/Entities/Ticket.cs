using System;
using System.Collections.Generic;
using System.Text;
using TicketManagement.Domain.Enums;

namespace TicketManagement.Domain.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;

        public TicketPriority Priority { get; set; }
        public TicketStatus Status { get; set; }

        public string CreatedByUserId { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt {get; set;}

    }
}
