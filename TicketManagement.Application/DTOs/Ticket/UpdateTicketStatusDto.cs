using System.ComponentModel.DataAnnotations;
using TicketManagement.Domain.Enums;

namespace TicketManagement.Application.DTOs.Ticket
{
    public class UpdateTicketStatusDto
    {
        //Only allowed values: Open/Closed
        [Required]
        public TicketStatus Status { get; set; }
    }
}
