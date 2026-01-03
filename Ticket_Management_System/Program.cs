using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using TicketManagement.API.Middleware;
using TicketManagement.Application.Helpers;
using TicketManagement.Application.Interfaces;
using TicketManagement.Application.Services;
using TicketManagement.Domain.Entities;
using TicketManagement.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// -------------------- CORS --------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// -------------------- DATABASE --------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// -------------------- SERVICES --------------------
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<JwtTokenHelper>();
builder.Services.AddScoped<ITicketService, TicketService>();

// -------------------- JWT AUTHENTICATION --------------------
var jwtKey = builder.Configuration["JwtSettings:Key"]!;
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"]!;
var jwtAudience = builder.Configuration["JwtSettings:Audience"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,

        ValidIssuer = jwtIssuer,
        ValidAudiences = new[] { jwtAudience },
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role
    };

    // Only enable verbose JWT event logging in Development
    if (builder.Environment.IsDevelopment())
    {
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"\n❌ JWT AUTHENTICATION FAILED");
                Console.WriteLine($"   Exception: {context.Exception.GetType().Name}");
                Console.WriteLine($"   Message: {context.Exception.Message}");
                if (context.Exception.InnerException != null)
                {
                    Console.WriteLine($"   Inner: {context.Exception.InnerException.Message}");
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine($"\n✅ JWT TOKEN VALIDATED SUCCESSFULLY");
                Console.WriteLine($"   Claims Count: {context.Principal?.Claims.Count()}");
                return Task.CompletedTask;
            }
        };
    }
});

// -------------------- AUTHORIZATION --------------------
builder.Services.AddAuthorization();

// -------------------- IDENTITY (JWT FRIENDLY) --------------------
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// -------------------- CONTROLLERS --------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

// -------------------- MIDDLEWARE --------------------
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Register debug middleware only in Development
if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<AuthDebugMiddleware>();
}

app.UseHttpsRedirection();
app.UseRouting();

// ✅ CRITICAL ORDER: CORS → Authentication → Authorization
app.UseCors("AllowAll");
app.UseAuthentication();    
app.UseAuthorization();

app.MapControllers();

app.Run();
