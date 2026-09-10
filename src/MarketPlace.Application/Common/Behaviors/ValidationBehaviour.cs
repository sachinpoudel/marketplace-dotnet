using FluentValidation;
using MediatR;

namespace MarketPlace.Application.Common.Behaviors;


public sealed class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public readonly IEnumerable<IValidator<TRequest>> _validators;
    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next();

        var validationContext = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(validator => validator.ValidateAsync(validationContext, cancellationToken)));
        var failures = validationResults.SelectMany(result => result.Errors).Where(failure => failure is not null).ToArray();

        if (failures.Length > 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
