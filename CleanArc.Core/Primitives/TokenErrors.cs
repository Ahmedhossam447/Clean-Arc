namespace CleanArc.Core.Primitives
{
    public static class TokenErrors
    {
        public static readonly Error InvalidToken = new(
            "Token.Invalid",
            "The refresh token is invalid.");

        public static readonly Error ExpiredToken = new(
            "Token.Expired",
            "The refresh token has expired.");

        public static readonly Error RevokedToken = new(
            "Token.Revoked",
            "The refresh token has been revoked.");

        public static readonly Error UserNotFound = new(
            "Token.UserNotFound",
            "The user associated with this token was not found.");
    }
}
