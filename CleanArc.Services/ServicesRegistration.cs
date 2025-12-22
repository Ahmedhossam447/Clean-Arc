using CleanArc.Core.Interfaces;
using CleanArc.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArc.Services
{
    public static class ServicesRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<ICarServices, CarServices>();
        }
    }
}
