    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using TicketManagement.Domain.Entities;

    namespace TicketManagement.Infrastructure.Data
    {               
        public class AppDbContext : IdentityDbContext<ApplicationUser>
        {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

            public DbSet<Ticket> Tickets { get; set; }
            public DbSet<RefreshToken> RefreshTokens { get; set; }

    }
}

    //👉 This connects Identity + Tickets in one DB.