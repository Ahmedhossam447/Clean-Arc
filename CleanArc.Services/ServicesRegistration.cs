using CleanArc.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArc.Services
{
    public static class ServicesRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAnimalServices, AnimalServices>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
        }
    }
}
