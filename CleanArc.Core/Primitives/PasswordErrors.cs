namespace CleanArc.Core.Primitives
{
    public static class PasswordErrors
    {
        public static readonly Error ResetFailed = new(
            "Password.ResetFailed",
            "Password reset failed. Invalid or expired token.");

        public static readonly Error UserNotFound = new(
            "Password.UserNotFound",
            "No user found with this email address.");

        public static readonly Error InvalidToken = new(
            "Password.InvalidToken",
            "The reset token is invalid or has expired.");

        public static readonly Error InvalidCurrentPassword = new(
            "Password.InvalidCurrentPassword",
            "The current password is incorrect.");

        public static readonly Error ChangeFailed = new(
            "Password.ChangeFailed",
            "Password change failed. Please check your current password and try again.");
    }
}
