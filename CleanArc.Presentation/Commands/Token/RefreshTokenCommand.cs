using CleanArc.Application.Contracts.Responses.Token;
using CleanArc.Core.Primitives;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Application.Commands.Token
{
    public class RefreshTokenCommand : IRequest<Result<RefreshTokenResponse>>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
