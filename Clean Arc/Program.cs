using CleanArc.Application;
using CleanArc.Infrastructure;
using CleanArc.Services;
using huzcodes.Extensions.Exceptions;
using Microsoft.EntityFrameworkCore;

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
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();

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
            app.AddExceptionHandlerExtension();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
