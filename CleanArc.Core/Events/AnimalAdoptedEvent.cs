using MediatR;

namespace CleanArc.Core.Events
{
    public class AnimalAdoptedEvent 
    {
        public int AnimalId { get; set; }
        public string AnimalName { get; set; }
        public string AnimalType { get; set; }
        public string AdopterId { get; set; }
        public string AdopterName { get; set; }
        public string AdopterEmail { get; set; }
        public string OwnerId { get; set; }
        public string OwnerEmail { get; set; }
        public DateTime AdoptedAt { get; set; }


    }
}
