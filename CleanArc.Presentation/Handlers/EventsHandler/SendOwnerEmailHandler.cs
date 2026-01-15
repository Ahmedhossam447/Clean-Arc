using CleanArc.Core.Events;
using CleanArc.Core.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Application.Handlers.EventsHandler
{
    public class SendOwnerEmailHandler : INotificationHandler<AnimalAdoptedEvent>
    {
        private readonly IEmailService _emailService;
        public SendOwnerEmailHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }
        public Task Handle(AnimalAdoptedEvent notification, CancellationToken cancellationToken)
        {
            var subject = $"Your pet {notification.AnimalName} has been adopted! 🐾";
            var body = $@"
                <h2>Dear Pet Owner,</h2>
                <p>We are excited to inform you that your beloved pet <strong>{notification.AnimalName}</strong> has found a new loving home!</p>
                <p><strong>Adoption Details:</strong></p>
                <ul>
                    <li><strong>Pet Name:</strong> {notification.AnimalName}</li>
                    <li><strong>Pet Type:</strong> {notification.AnimalType}</li>
                    <li><strong>Adoption Date:</strong> {notification.AdoptedAt:MMMM dd, yyyy}</li>
                </ul>
                <p>Thank you for entrusting us with the care of {notification.AnimalName}. We wish you all the best!</p>
                <p>If you have any questions, please don't hesitate to contact us.</p>
                <p>Sincerely,<br/>The HappyPaws Team 🐾</p>
            ";
            return _emailService.SendEmailAsync(notification.OwnerEmail, subject, body, isHtml: true);

        }
    }
}
