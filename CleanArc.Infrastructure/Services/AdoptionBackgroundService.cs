using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;


namespace CleanArc.Infrastructure.Services
{
    public class AdoptionBackgroundService : IAdoptionBackgroundService
    {
        private readonly IUnitOfWork _Uow;
        private readonly INotificationService _notificationService;
        public AdoptionBackgroundService(IUnitOfWork uow,INotificationService service )
        {
            _Uow = uow;
            _notificationService = service;

        }
        public async Task ProcessRejectedRequestsAsync(int animalId, int AcceptedReqId)
        {
            var rejectedRequests = await _Uow.Repository<Request>().GetAsync(ar => ar.AnimalId == animalId && ar.Id != AcceptedReqId && ar.Status == "Pending");
            var rejectedRequestsUsersIds = rejectedRequests.Select(r => r.RequesterId).ToList();
            var notifications = rejectedRequestsUsersIds.Select(userId => new Notification
            {
                UserId = userId,
                Message = $"Your adoption request for animal with ID {animalId} has been rejected.",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            }).ToList();
            await _Uow.Repository<Notification>().AddRangeAsync(notifications);
             _Uow.Repository<Request>().RemoveRange(rejectedRequests);
            await _Uow.SaveChangesAsync();
            if (rejectedRequestsUsersIds.Count > 0)
                await _notificationService.SendNotificationAsync(rejectedRequestsUsersIds,"ReceiveNotification", "Your adoption request was rejected.");

        }
    }
}
