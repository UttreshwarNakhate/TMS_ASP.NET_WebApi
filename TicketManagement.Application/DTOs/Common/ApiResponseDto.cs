namespace TicketManagement.Application.DTOs.Common
{
    public class ApiResponseDto<T>
    {
        public int StatusCode { get; set; }
        public string StatusDesc { get; set; } = string.Empty;
        public string StatusType { get; set; } = string.Empty;
        public T? Details { get; set; }
    }
}
