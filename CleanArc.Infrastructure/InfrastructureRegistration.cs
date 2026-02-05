using CleanArc.Core.Interfaces;
using CleanArc.Infrastructure.Identity;
using CleanArc.Infrastructure.Persistence;
using CleanArc.Infrastructure.Persistence.Data;
using CleanArc.Infrastructure.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CleanArc.Application.Consumers;

namespace CleanArc.Infrastructure
{
    public static class InfrastructureRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services, string connection, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connection));

            // Register Identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("RedisConnection");
                options.InstanceName = "CleanArc";
            });

            // Register repositories
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IAnimalRepository, AnimalRepository>();
            services.AddScoped<IRequestRepository, RequestRepository>();
            services.AddAuthentication(option=>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(Options=>
            {
                Options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")))
                };
            });
            services.AddMassTransit(x =>
            {
                // Consumer 1: Email notifications
                x.AddConsumer<AnimalAdoptConsumer>();
                
                // Consumer 2: Audit logging
                x.AddConsumer<LogAdoptionAuditConsumer>();
                
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    // Queue for email notifications
                    cfg.ReceiveEndpoint("animal-adopted-notifications", e =>
                    {
                        e.ConfigureConsumer<AnimalAdoptConsumer>(context);
                    });
                    
                    // Separate queue for audit logging (both consumers receive the same event)
                    cfg.ReceiveEndpoint("animal-adopted-audit", e =>
                    {
                        e.ConfigureConsumer<LogAdoptionAuditConsumer>(context);
                    });
                    
                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });
            });

            // Register SignalR
            services.AddSignalR();
            services.AddSingleton<Microsoft.AspNetCore.SignalR.IUserIdProvider, Hubs.NameUserIdProvider>();
            
            // Register external services (Infrastructure layer)
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBackgroundJobService,BackgroundJobService>();
            services.AddScoped<IImageService, S3ImageService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
