using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.Domain.Entities
{
    public  class RefreshToken
    {
        public int Id { get; set; }

        // Actual refresh token string
        public string Token { get; set; } = null!;

        // FK to AspNetUsers.Id
        public string UserId { get; set; } = null!;

        // Expiry time of refresh token
        public DateTime ExpiresAt { get; set; }

        // To support logout / rotation
        public bool IsRevoked { get; set; }
    }
}
