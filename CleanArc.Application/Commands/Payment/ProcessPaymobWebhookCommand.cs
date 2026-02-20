using CleanArc.Core.Dtos;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Commands.Payment
{
    public class ProcessPaymobWebhookCommand :IRequest<Result<bool>>
    {
        public PaymobWebhookDto PaymobWebhook { get; set; }
    }
}
