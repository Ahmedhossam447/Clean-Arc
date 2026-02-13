using CleanArc.Core.Events;
using CleanArc.Core.Interfaces;
using CleanArc.Core;
using MassTransit;

namespace CleanArc.Infrastructure.Consumers
{
    public class AnimalAdoptConsumer : IConsumer<AnimalAdoptedEvent>
    {
        private readonly IEmailService _emailService;
        public AnimalAdoptConsumer(IEmailService emailService)
        {
            _emailService = emailService;
        }
        public async Task Consume(ConsumeContext<AnimalAdoptedEvent> context)
        {
            var message = context.Message;

            var adopterSubject = EmailTemplates.GetAdoptionConfirmationSubject(message.AnimalName);
            var adopterBody = EmailTemplates.GetAdoptionConfirmationBody(
                message.AdopterName,
                message.AnimalName,
                message.AnimalType,
                message.AdoptedAt);

            await _emailService.SendEmailAsync(
                message.AdopterEmail,
                adopterSubject,
                adopterBody,
                isHtml: true
            );

            var ownerSubject = EmailTemplates.GetAdoptionOwnerNotificationSubject(message.AnimalName);
            var ownerBody = EmailTemplates.GetAdoptionOwnerNotificationBody(
                message.AnimalName,
                message.AnimalType,
                message.AdoptedAt);

            await _emailService.SendEmailAsync(
                message.OwnerEmail,
                ownerSubject,
                ownerBody,
                isHtml: true
            );
        }
    }
}
