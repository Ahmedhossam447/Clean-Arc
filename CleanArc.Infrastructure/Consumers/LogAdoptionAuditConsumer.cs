using CleanArc.Core.Entities;
using CleanArc.Core.Events;
using CleanArc.Core.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CleanArc.Infrastructure.Consumers
{
    /// <summary>
    /// Consumer 2: Logs adoption events to the audit log for compliance and historical tracking.
    /// This runs independently from the email notification consumer.
    /// </summary>
    public class LogAdoptionAuditConsumer : IConsumer<AnimalAdoptedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LogAdoptionAuditConsumer> _logger;

        public LogAdoptionAuditConsumer(
            IUnitOfWork unitOfWork,
            ILogger<LogAdoptionAuditConsumer> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<AnimalAdoptedEvent> context)
        {
            var message = context.Message;
            
            _logger.LogInformation(
                "Creating audit log for Animal {AnimalId} adoption by {AdopterId}", 
                message.AnimalId, 
                message.AdopterId);

            var auditLog = new AdoptionAuditLog
            {
                AnimalId = message.AnimalId,
                AnimalName = message.AnimalName,
                AnimalType = message.AnimalType,
                AdopterId = message.AdopterId,
                AdopterName = message.AdopterName,
                AdopterEmail = message.AdopterEmail,
                PreviousOwnerId = message.OwnerId,
                PreviousOwnerEmail = message.OwnerEmail,
                AdoptedAt = message.AdoptedAt,
                LoggedAt = DateTime.UtcNow,
                ProcessedBy = "MassTransit-AuditConsumer"
            };

            try
            {
                await _unitOfWork.Repository<AdoptionAuditLog>().AddAsync(auditLog);
                await _unitOfWork.SaveChangesAsync();
                
                _logger.LogInformation(
                    "Audit log created successfully for Animal {AnimalId}, AuditLog ID: {AuditLogId}", 
                    message.AnimalId,
                    auditLog.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Failed to create audit log for Animal {AnimalId}", 
                    message.AnimalId);
                throw; // Rethrow so MassTransit can handle retry logic
            }
        }
    }
}
