using CleanArc.Application.Dtos;
using CleanArc.Core.Primitives;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Application.Commands.Payment
{
    public class ProcessPaymobWebhookCommand :IRequest<Result<bool>>
    {
        public PaymobWebhookDto PaymobWebhook { get; set; }
    }
}
