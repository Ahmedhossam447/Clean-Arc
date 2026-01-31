using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Core.Primitives;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Application.Commands.Auth
{
    public class LogoutCommand:IRequest<Result<LogoutResponse>>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
