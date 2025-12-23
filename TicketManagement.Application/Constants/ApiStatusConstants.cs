namespace TicketManagement.Application.Constants
{
    public static class ApiStatusConstants
    {
        public const string SuccessType = "S";
        public const string ErrorType = "E";

        public const int SuccessCode = 200;
        public const int BadRequestCode = 400;
        public const int NotFoundCode = 404;

        public const string UserCreated = "User registered successfully";
        public const string UserAlreadyExists = "User already exists";
        public const string RegistrationFailed = "User registration failed";
        public const string WrongEmail = "Invalid email address";
        public const string WrongPassword = "Password is incorrect";
        public const string LoginSuccess = "User logged-in successful";
    }
}
