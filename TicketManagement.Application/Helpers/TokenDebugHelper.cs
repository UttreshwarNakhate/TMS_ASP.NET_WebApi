using System.IdentityModel.Tokens.Jwt;

namespace TicketManagement.Application.Helpers
{
    public static class TokenDebugHelper
    {
        public static void LogTokenDetails(string token)
        {
            try
            {
                Console.WriteLine("\n========== JWT TOKEN DEBUG INFO ==========");
                
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                {
                    Console.WriteLine("❌ TOKEN DECODE FAILED: Token is not a valid JWT");
                    return;
                }

                Console.WriteLine($"✓ Token Valid: Yes");
                Console.WriteLine($"✓ Algorithm: {jwtToken.Header["alg"]}");
                Console.WriteLine($"✓ Type: {jwtToken.Header["typ"]}");
                Console.WriteLine($"✓ Issued At: {jwtToken.IssuedAt:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine($"✓ Expires At: {jwtToken.ValidTo:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine($"✓ Issuer: {jwtToken.Issuer}");
                Console.WriteLine($"✓ Audience: {string.Join(", ", jwtToken.Audiences)}");
                
                Console.WriteLine("\n--- CLAIMS IN TOKEN ---");
                foreach (var claim in jwtToken.Claims)
                {
                    Console.WriteLine($"  • Type: {claim.Type}");
                    Console.WriteLine($"    Value: {claim.Value}");
                }
                
                Console.WriteLine($"\n--- TOKEN (First 80 chars) ---");
                Console.WriteLine($"  {token.Substring(0, Math.Min(80, token.Length))}...");
                Console.WriteLine("=========================================\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR IN TokenDebugHelper: {ex.Message}");
            }
        }

        private static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}