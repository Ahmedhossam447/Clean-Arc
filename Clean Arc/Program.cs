using Clean_Arc.Hubs;
using Clean_Arc.Middleware;
using CleanArc;
using CleanArc.Application;
using CleanArc.Core.Interfaces;
using CleanArc.Infrastructure;
using CleanArc.Services;
using huzcodes.Extensions.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

namespace Clean_Arc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            var connection = builder.Configuration.GetSection("ConnectionStrings:AnimalConnection").Value;
            builder.Services.AddInfrastructureServices(connection,builder.Configuration);
            builder.Services.AddPresentationRegistration();


            builder.Services.AddServices();
            builder.Services.AddSignalR();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<GlobalExceptionHandler>();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CleanArc API",
                    Version = "v1"
                });

                // Order endpoints: POST, GET, PUT, DELETE (strictly grouped by method)
                options.OrderActionsBy(desc =>
                {
                    var methodOrder = desc.HttpMethod switch
                    {
                        "POST" => "0",
                        "GET" => "1", 
                        "PUT" => "2",
                        "PATCH" => "3",
                        "DELETE" => "4",
                        _ => "5"
                    };
                    return $"{desc.ActionDescriptor.RouteValues["controller"]}~{methodOrder}";
                });

                // Add JWT Authentication
                const string schemeName = "Bearer";
                
                options.AddSecurityDefinition(schemeName, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token"
                });

                // New Swashbuckle 10.x syntax
                options.AddSecurityRequirement(document =>
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference(schemeName, document)] = new List<string>()
                    }
                );
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CleanARC API V1");
                c.RoutePrefix = "swagger"; // Swagger UI will be available at /swagger
                c.DisplayRequestDuration();
            });
            app.UseMiddleware<GlobalExceptionHandler>();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            app.MapHub<ChatHub>("/chatHub");

            app.Run();
        }
    }
}
