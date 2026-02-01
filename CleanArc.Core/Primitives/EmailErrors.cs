namespace CleanArc.Core.Primitives
{
    public static class EmailErrors
    {
        public static readonly Error ConfirmationFailed = new(
            "Email.ConfirmationFailed",
            "Email confirmation failed. Invalid or expired token.");

        public static readonly Error AlreadyConfirmed = new(
            "Email.AlreadyConfirmed",
            "This email has already been confirmed.");

        public static readonly Error NotConfirmed = new(
            "Email.NotConfirmed",
            "Please confirm your email address before logging in.");

        public static readonly Error UserNotFound = new(
            "Email.UserNotFound",
            "No user found with this email address.");

        public static readonly Error InvalidToken = new(
            "Email.InvalidToken",
            "The confirmation token is invalid or has expired.");
    }
}
