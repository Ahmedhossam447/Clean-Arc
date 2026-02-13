namespace CleanArc.Core.Interfaces
{
    public interface IAdoptionBackgroundService
    {
        Task ProcessRejectedRequestsAsync(int animalId, int AcceptedReqId);
    }
}
