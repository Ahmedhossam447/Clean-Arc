using CleanArc.Core.Events;
using CleanArc.Core.Interfaces;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Application.Consumers
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
            var subject = $"Congratulations! You've adopted {context.Message.AnimalName}! 🎉";

            var body = $@"
                <h2>Dear {context.Message.AdopterName},</h2>
                <p>We're thrilled to let you know that your adoption request for <strong>{context.Message.AnimalName}</strong> has been approved!</p>
                <p><strong>Adoption Details:</strong></p>
                <ul>
                    <li><strong>Pet Name:</strong> {context.Message.AnimalName}</li>
                    <li><strong>Pet Type:</strong> {context.Message.AnimalType}</li>
                    <li><strong>Adoption Date:</strong> {context.Message.AdoptedAt:MMMM dd, yyyy}</li>
                </ul>
                <p>Thank you for choosing to adopt and giving {context.Message.AnimalName} a loving home!</p>
                <p>If you have any questions, feel free to reach out to us.</p>
                <p>With love,<br/>The HappyPaws Team 🐾</p>
            ";
            await _emailService.SendEmailAsync(
                context.Message.AdopterEmail,
                subject,
                body,
                isHtml: true
            );
            var subjectOwner = $"Your pet {context.Message.AnimalName} has been adopted! 🐾";
            var bodyOwner = $@"
                <h2>Dear Pet Owner,</h2>
                <p>We are excited to inform you that your beloved pet <strong>{context.Message.AnimalName}</strong> has found a new loving home!</p>
                <p><strong>Adoption Details:</strong></p>
                <ul>
                    <li><strong>Pet Name:</strong> {context.Message.AnimalName}</li>
                    <li><strong>Pet Type:</strong> {context.Message.AnimalType}</li>
                    <li><strong>Adoption Date:</strong> {context.Message.AdoptedAt:MMMM dd, yyyy}</li>
                </ul>
                <p>Thank you for entrusting us with the care of {context.Message.AnimalName}. We wish you all the best!</p>
                <p>If you have any questions, please don't hesitate to contact us.</p>
                <p>Sincerely,<br/>The HappyPaws Team 🐾</p>
            ";
            await _emailService.SendEmailAsync(
                context.Message.OwnerEmail,
                subjectOwner,
                bodyOwner,
                isHtml: true
            );
        }
    }
}
