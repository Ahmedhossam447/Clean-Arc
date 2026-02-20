using CleanArc.Application.Commands.Payment;
using CleanArc.Core.Dtos;
using CleanArc.API.Extensions;
using CleanArc.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CleanArc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PaymentController> _logger;
        
        public PaymentController(IMediator mediator, ILogger<PaymentController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromQuery] string hmac)
        {
            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync();
            
            _logger.LogInformation("Webhook received. Content-Type: {ContentType}, Body length: {Length}", 
                Request.ContentType, rawBody.Length);

            var dto = JsonSerializer.Deserialize<PaymobWebhookDto>(rawBody);
            if (dto == null)
                return BadRequest("Invalid webhook payload");

            dto.Hmac = hmac;
            var result = await _mediator.Send(new ProcessPaymobWebhookCommand { PaymobWebhook = dto });

            if (result.IsFailure)
            {
                _logger.LogWarning("Webhook processing failed: {Error}", result.Error.Description);

                // Reject invalid signatures — Paymob should not retry these
                if (result.Error == PaymentTransaction.Errors.InvalidSignature)
                    return Unauthorized(new { error = result.Error.Description });

                // For everything else (not found, already processed, failed), 
                // return 200 so Paymob stops retrying
                return Ok(new { status = "received", message = result.Error.Description });
            }

            return Ok(new { status = "success" });
        }
    }
}

