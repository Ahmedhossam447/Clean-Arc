using CleanArc.Core.Interfaces;
using CleanArc.Infrastructure.Persistence;
using CleanArc.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CleanArc.Infrastructure
{
    public static class InfrastructureRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services,string connection)
        {
            services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connection));
            // Register repositories
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

        }
    }
}
