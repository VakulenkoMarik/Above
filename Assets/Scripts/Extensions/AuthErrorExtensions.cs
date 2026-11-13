using Firebase.Auth;

namespace Extensions
{
    public static class AuthErrorExtensions 
    {
        public static string GetErrorText(this AuthError error, string defaultMessage) {
            string failedMessage = error switch
            {
                AuthError.InvalidEmail => "Email is invalid",
                AuthError.MissingEmail => "Email is missing",
                AuthError.WrongPassword => "Wrong Password",
                AuthError.MissingPassword => "Password is missing",
                AuthError.Cancelled => "The action was cancelled",
                AuthError.TooManyRequests => "Too many requests",
                AuthError.InvalidRecipientEmail => "The email you entered is invalid",
                
                _ => defaultMessage
            };

            return failedMessage;
        }
    }
}