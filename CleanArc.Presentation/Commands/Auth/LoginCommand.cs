using CleanArc.Application.Contracts.Responses.Auth;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Application.Commands.Auth
{
    public class LoginCommand :IRequest<LoginResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
