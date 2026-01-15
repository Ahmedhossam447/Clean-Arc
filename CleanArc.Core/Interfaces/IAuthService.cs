using CleanArc.Core.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CleanArc.Core.Interfaces
{
    public interface IAuthService
    {
        public Task<(bool Succeeded, string[] Errors)> RegisterUserAsync(string username, string password,string email);
        public Task<ApplicationUser> LoginUserAsync(string email, string password);
    }
}
