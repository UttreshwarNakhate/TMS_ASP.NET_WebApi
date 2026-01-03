using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using TicketManagement.Application.Helpers;

namespace TicketManagement.API.Controllers
{
    [ApiController]
    [Route("api/debug")]
    public class DebugController : ControllerBase
    {
        /// <summary>
        /// Endpoint to verify JWT token is being parsed correctly
        /// </summary>
        [HttpGet("token-info")]
        [Authorize]
        public IActionResult GetTokenInfo()
        {
            Console.WriteLine("\n========== DEBUG: Token Info Endpoint ==========");

            var authHeader = Request.Headers["Authorization"].ToString();
            Console.WriteLine($"1️⃣ Authorization Header Present: {!string.IsNullOrEmpty(authHeader)}");
            
            if (!string.IsNullOrEmpty(authHeader))
            {
                Console.WriteLine($"   • Header Value: {authHeader.Substring(0, Math.Min(80, authHeader.Length))}...");
            }

            // Check if user is authenticated
            Console.WriteLine($"2️⃣ User.Identity?.IsAuthenticated: {User?.Identity?.IsAuthenticated}");
            Console.WriteLine($"   • User.Identity?.Name: {User?.Identity?.Name ?? "NULL"}");

            // Count claims
            var claimsList = User.Claims.ToList();
            Console.WriteLine($"3️⃣ Total Claims Count: {claimsList.Count}");

            if (claimsList.Count > 0)
            {
                Console.WriteLine("\n--- RAW CLAIMS ---");
                foreach (var claim in claimsList)
                {
                    Console.WriteLine($"  • Type: {claim.Type}");
                    Console.WriteLine($"    Value: {claim.Value}");
                }
            }
            else
            {
                Console.WriteLine("❌ NO CLAIMS FOUND!");
            }

            // Extract userId using different methods
            Console.WriteLine("\n--- USERID EXTRACTION ATTEMPTS ---");
            
            var userId1 = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"  Method 1 (ClaimTypes.NameIdentifier): {userId1 ?? "NULL"}");

            var userId2 = User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            Console.WriteLine($"  Method 2 (Full URI): {userId2 ?? "NULL"}");

            var userId3 = User.Claims.FirstOrDefault(c => c.Type.EndsWith("nameidentifier", StringComparison.OrdinalIgnoreCase))?.Value;
            Console.WriteLine($"  Method 3 (Case-insensitive): {userId3 ?? "NULL"}");

            var email = User.FindFirstValue(ClaimTypes.Email);
            Console.WriteLine($"  Email: {email ?? "NULL"}");

            Console.WriteLine("===========================================\n");

            return Ok(new
            {
                success = !string.IsNullOrEmpty(userId1),
                userId = userId1,
                email = email,
                allClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList(),
                debugInfo = new
                {
                    userIdMethod1 = userId1,
                    userIdMethod2 = userId2,
                    userIdMethod3 = userId3,
                    totalClaimsCount = User.Claims.Count()
                }
            });
        }

        /// <summary>
        /// Test endpoint without authorization
        /// </summary>
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "API is working" });
        }

        /// <summary>
        /// Verify token validation parameters
        /// </summary>
        [HttpPost("validate-token")]
        public IActionResult ValidateToken([FromBody] string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                {
                    return BadRequest(new { error = "Invalid token format" });
                }

                return Ok(new
                {
                    isValid = true,
                    issuer = jwtToken.Issuer,
                    audience = string.Join(", ", jwtToken.Audiences),
                    expiresAt = jwtToken.ValidTo,
                    claims = jwtToken.Claims.Select(c => new { c.Type, c.Value }).ToList()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}