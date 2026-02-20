using CleanArc.Application.Pipeline_Behaviour;
using CleanArc.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using huzcodes.Extensions.Exceptions;
using CleanArc.Application.Pipeline_Behaviour;


namespace CleanArc.Application
{
    public static class AddApplicationRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddApplicationRegistration).Assembly));
            services.AddFluentValidation(typeof(AddApplicationRegistration));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPipeline<,>));
        }
    }
}
