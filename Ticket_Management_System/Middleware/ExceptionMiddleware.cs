using System.Net;
using System.Text.Json;
using TicketManagement.Application.Constants;
using TicketManagement.Application.DTOs.Common;

namespace TicketManagement.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        // RequestDelegate represents the next middleware in pipeline
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        // This method is called for every HTTP request
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Pass request to next middleware/controller
                await _next(context);
            }
            catch (Exception ex)
            {
                // If any exception occurs, it will come here
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            // Set response content type as JSON
            context.Response.ContentType = "application/json";

            // Internal Server Error (500)
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Create standard API response
            var response = new ApiResponseDto<object>
            {
                StatusCode = context.Response.StatusCode,
                StatusDesc = "An unexpected error occurred",
                StatusType = ApiStatusConstants.ErrorType,
                Details = exception.Message // for debugging (remove in prod)
            };

            // Convert response object to JSON
            var jsonResponse = JsonSerializer.Serialize(response);

            // Write response to HTTP output
            await context.Response.WriteAsync(jsonResponse);
        }





    }

}
