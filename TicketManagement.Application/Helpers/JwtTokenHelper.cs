using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TicketManagement.Domain.Entities;
using System.Security.Claims;
using System.Text;


namespace TicketManagement.Application.Helpers
{
    public class JwtTokenHelper
    {
       //variable declaration
        private readonly IConfiguration _configuration;

        //constructor
        // IConfiguration reads appsettings.json
        public JwtTokenHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        //Method to generate jwt token
        // Generate JWT token for logged-in user
        public string GenerateToken(ApplicationUser user)
        {
            // Claims = payload (like JWT payload in Node)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            //Secret key from appsettings.json
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtSettings:key"]!)
                );

            //signing credentials (HMAC SHA256)
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create token
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    Convert.ToDouble(_configuration["JwtSettings:ExpiryMinutes"])
                ),
                signingCredentials: creds
            );

            // Convert token object to string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
