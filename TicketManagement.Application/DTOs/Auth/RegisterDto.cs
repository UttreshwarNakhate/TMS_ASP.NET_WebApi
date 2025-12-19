using System.ComponentModel.DataAnnotations;


namespace TicketManagement.Application.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(6)]
        public string Password { get; set; } = null!;
    }

    //👉 DTO protects your Entity
    //👉 Validation happens before DB call
}
