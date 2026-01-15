namespace CleanArc.Application.Contracts.Responses.Request
{
    public class CreateRequestResponse
    {
        public int Reqid { get; set; }
        public string? Userid { get; set; }
        public string? Useridreq { get; set; }
        public int AnimalId { get; set; }
        public string? Status { get; set; }
    }
}
