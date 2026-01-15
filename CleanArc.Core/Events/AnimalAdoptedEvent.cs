using MediatR;

namespace CleanArc.Core.Events
{
    public class AnimalAdoptedEvent : INotification
    {
        public int AnimalId { get; }
        public string AnimalName { get; }
        public string AnimalType { get; }
        public string AdopterId { get; }
        public string AdopterName { get; }
        public string AdopterEmail { get; }
        public string OwnerId { get; }
        public string OwnerEmail { get; }
        public DateTime AdoptedAt { get; }

        public AnimalAdoptedEvent(
            int animalId,
            string animalName,
            string animalType,
            string adopterId,
            string adopterName,
            string adopterEmail,
            string ownerId,
            string ownerEmail)
        {
            AnimalId = animalId;
            AnimalName = animalName;
            AnimalType = animalType;
            AdopterId = adopterId;
            AdopterName = adopterName;
            AdopterEmail = adopterEmail;
            OwnerId = ownerId;
            OwnerEmail = ownerEmail;
            AdoptedAt = DateTime.UtcNow;
        }
    }
}
