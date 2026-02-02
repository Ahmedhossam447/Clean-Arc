using CleanArc.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArc.Services
{
    public static class ServicesRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            // Note: All services have been moved:
            // - IAnimalServices -> Registered in AddPresentationRegistration (Application layer)
            // - IAuthService, IUserService -> Registered in InfrastructureRegistration (Infrastructure layer)
            // - IEmailService, ITokenService -> Registered in InfrastructureRegistration (Infrastructure layer)
            // This class can be removed once Program.cs is updated
        }
    }
}
