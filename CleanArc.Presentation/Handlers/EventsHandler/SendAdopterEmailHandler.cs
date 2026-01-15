using CleanArc.Core.Events;
using CleanArc.Core.Interfaces;
using MediatR;

namespace CleanArc.Application.Handlers.EventsHandler
{
    public class SendAdopterEmailHandler : INotificationHandler<AnimalAdoptedEvent>
    {
        private readonly IEmailService _emailService;

        public SendAdopterEmailHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task Handle(AnimalAdoptedEvent notification, CancellationToken cancellationToken)
        {
            var subject = $"Congratulations! You've adopted {notification.AnimalName}! 🎉";
            
            var body = $@"
                <h2>Dear {notification.AdopterName},</h2>
                <p>We're thrilled to let you know that your adoption request for <strong>{notification.AnimalName}</strong> has been approved!</p>
                <p><strong>Adoption Details:</strong></p>
                <ul>
                    <li><strong>Pet Name:</strong> {notification.AnimalName}</li>
                    <li><strong>Pet Type:</strong> {notification.AnimalType}</li>
                    <li><strong>Adoption Date:</strong> {notification.AdoptedAt:MMMM dd, yyyy}</li>
                </ul>
                <p>Thank you for choosing to adopt and giving {notification.AnimalName} a loving home!</p>
                <p>If you have any questions, feel free to reach out to us.</p>
                <p>With love,<br/>The HappyPaws Team 🐾</p>
            ";

            await _emailService.SendEmailAsync(notification.AdopterEmail, subject, body, isHtml: true);
        }
    }
}
