namespace CleanArc.Application.Contracts.Responses.Animal
{
    public class GetAllAnimalsResponse
    {
        public List<ReadAnimalResponse> Animals { get; set; } = new List<ReadAnimalResponse>();
    }
}
