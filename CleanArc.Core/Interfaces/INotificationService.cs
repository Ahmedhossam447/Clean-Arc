namespace CleanArc.Core.Interfaces
{
    public interface INotificationService
    {
       public Task SendNotificationAsync(List<string> userId, string type, object data);
        public Task SendNotificationToUserAsync(string userId, string type, object data);
    }
}
