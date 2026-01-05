namespace TicketManagement.Application.Constants
{
    public static class ApiStatusConstants
    {
        public const string SuccessType = "S";
        public const string ErrorType = "E";

        public const int SuccessCode = 200;
        public const int CreatedCode = 201;
        public const int BadRequestCode = 400;
        public const int NotFoundCode = 404;

        public const string UserCreated = "User registered successfully";
        public const string UserAlreadyExists = "User already exists";
        public const string RegistrationFailed = "User registration failed";
        public const string WrongEmail = "Invalid email address";
        public const string WrongPassword = "Password is incorrect";
        public const string LoginSuccess = "User logged-in successful";


        public const string TicketCreated = "Ticket created successfully";
        public const string TicketsFetched = "Tickets fetched successfully";
        public const string TicketsNotFound = "Ticket not found";
        public const string TicketDetailsFetched = "Ticket details fetched successfully";
        public const string StatusAlreadyExists = "Ticket already in this status ";
        public const string StatusUpdate = "Ticket status updated successfully";    }
}
