using Clean_Arc.Application.Pipline_Behaviour;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using huzcodes.Extensions.Exceptions;
using CleanArc.Application.Pipeline_Behaviour;
using MassTransit;

namespace CleanArc.Application
{
    public static class AddApplicationRegistration
    {
        public static void AddPresentationRegistration(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddApplicationRegistration).Assembly));
            services.AddFluentValidation(typeof(AddApplicationRegistration));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPipline<,>));
        }
    }
}
