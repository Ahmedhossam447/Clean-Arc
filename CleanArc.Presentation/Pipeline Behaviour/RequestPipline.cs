using FluentValidation;
using MediatR;

namespace Clean_Arc.Application.Pipline_Behaviour

{
    public class RequestPipline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public RequestPipline(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Validation
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults   
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();
            
            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }

            // Execute handler - exceptions will be handled by GlobalExceptionHandler
            return await next();
        }
    }
}
