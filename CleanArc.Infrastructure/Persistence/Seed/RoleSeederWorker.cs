using CleanArc.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CleanArc.Infrastructure.Persistence.Seed
{
    public class RoleSeederWorker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RoleSeederWorker> _logger;

        public RoleSeederWorker(IServiceProvider serviceProvider, ILogger<RoleSeederWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Seeding roles...");

            using var scope = _serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await SeedRole(roleManager, "Shelter");
            await SeedRole(roleManager, "Admin");
            await SeedRole(roleManager, "User");

            _logger.LogInformation("Role seeding completed.");
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task SeedRole(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                    _logger.LogInformation("Role '{RoleName}' created successfully.", roleName);
                else
                    _logger.LogWarning("Failed to create role '{RoleName}': {Errors}", roleName,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
