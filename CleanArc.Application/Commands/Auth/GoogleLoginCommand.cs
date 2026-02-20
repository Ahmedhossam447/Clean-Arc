using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Core.Primitives;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Application.Commands.Auth
{
    public class GoogleLoginCommand: IRequest<Result<GoogleLoginResponse>>
    {
        public string TokenId { get; set; }
    }
}
