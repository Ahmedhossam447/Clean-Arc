namespace CleanArc.Application.Contracts.Responses.Auth
{
    public class ForgotPasswordResponse
    {
        public string Message { get; set; } = "If an account with this email exists, a password reset link has been sent.";
    }
}
