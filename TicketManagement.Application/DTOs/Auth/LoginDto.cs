
using System.ComponentModel.DataAnnotations;

namespace TicketManagement.Application.DTOs.Auth
{
    // DTO = Data coming from client (like req.body in Node.js)
    public class LoginDto
    {
        // Email is required
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        // Password is required
        [Required]
        public string Password { get; set; } = null!;
    }
}
