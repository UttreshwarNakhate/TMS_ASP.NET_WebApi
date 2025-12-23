using Microsoft.EntityFrameworkCore;
using TicketManagement.Infrastructure.Data;
using TicketManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using TicketManagement.Application.Interfaces;
using TicketManagement.Application.Services;
using TicketManagement.Application.Helpers;

var builder = WebApplication.CreateBuilder(args);

//Add database connection here
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Add AuthService here which us used for user authentication
builder.Services.AddScoped<IAuthService,AuthService>();

// Add JwtTokenHelper
builder.Services.AddScoped<JwtTokenHelper>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();

app.UseAuthorization();

app.UseHttpsRedirection();


app.MapControllers();




app.Run();
