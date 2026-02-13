namespace CleanArc.Application.Contracts.Responses.Auth
{
    public class RegisterResponse
    {
        public bool Succeeded { get; set; }
        public string[] Errors { get; set; }
    }
}
