using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Application.Contracts.Responses.Auth
{
    public class LoginResponse
    {
        public bool Succeeded { get; set; }
        public List<string> Errors { get; set; }
        public string Token { get; set; }
    }
}
