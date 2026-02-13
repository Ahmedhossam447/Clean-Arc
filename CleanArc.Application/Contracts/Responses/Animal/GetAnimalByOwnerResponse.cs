namespace CleanArc.Application.Contracts.Responses.Animal
{
    public class GetAnimalsByOwnerResponse
    {
        public List<ReadAnimalResponse> Animals { get; set; } = new List<ReadAnimalResponse>();
    }
}
