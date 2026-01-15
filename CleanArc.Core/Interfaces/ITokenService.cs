using CleanArc.Core.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Core.Interfaces
{
    public interface ITokenService
    {
       string GenerateAccessToken(ApplicationUser user);
    }
}
