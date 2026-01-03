using System.Security.Claims;

namespace TicketManagement.API.Middleware
{
    public class AuthDebugMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthDebugMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log all requests to debug endpoints
            if (context.Request.Path.StartsWithSegments("/api/debug") ||
                context.Request.Path.StartsWithSegments("/api/auth"))
            {
                Console.WriteLine($"\n{'=' * 60}");
                Console.WriteLine($"📌 REQUEST: {context.Request.Method} {context.Request.Path}");
                Console.WriteLine($"{'=' * 60}");

                // Log authorization header
                var authHeader = context.Request.Headers["Authorization"].ToString();
                Console.WriteLine($"✓ Authorization Header: {(string.IsNullOrEmpty(authHeader) ? "MISSING" : "PRESENT")}");

                if (!string.IsNullOrEmpty(authHeader))
                {
                    Console.WriteLine($"  • Value (first 80 chars): {authHeader.Substring(0, Math.Min(80, authHeader.Length))}...");
                }

                // Log body if POST/PUT
                if (context.Request.ContentLength > 0 &&
                    (context.Request.Method == "POST" || context.Request.Method == "PUT"))
                {
                    context.Request.EnableBuffering();
                    using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
                    {
                        var body = await reader.ReadToEndAsync();
                        Console.WriteLine($"✓ Request Body: {body.Substring(0, Math.Min(200, body.Length))}...");
                        context.Request.Body.Position = 0;
                    }
                }
            }

            await _next(context);

            // Log response for debug endpoints
            if (context.Request.Path.StartsWithSegments("/api/debug") ||
                context.Request.Path.StartsWithSegments("/api/auth"))
            {
                Console.WriteLine($"\n✓ RESPONSE Status: {context.Response.StatusCode}");
                Console.WriteLine($"{'=' * 60}\n");
            }
        }
    }
}