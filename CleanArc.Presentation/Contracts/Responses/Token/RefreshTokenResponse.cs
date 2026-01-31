using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Application.Contracts.Responses.Token
{
    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
