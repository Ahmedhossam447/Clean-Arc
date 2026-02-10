using CleanArc.Core.Primitives;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Application.Commands.Payment
{
    public class InitiatePaymentCommand :IRequest<Result<string>>
    {
        public int amount { get; set; }
    }
}
