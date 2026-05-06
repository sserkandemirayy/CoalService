using Application.Common.Models;
using FluentValidation;
using MediatR;

namespace Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var failures = (await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken))))
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                var errorMessage = string.Join("; ", failures.Select(f => f.ErrorMessage));

                // Result<T> beklenen generic tür ise Failure döndür
                var resultType = typeof(TResponse);
                if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var failureMethod = resultType.GetMethod("Failure", new[] { typeof(string) });
                    if (failureMethod != null)
                    {
                        return (TResponse)failureMethod.Invoke(null, new object[] { errorMessage })!;
                    }
                }

                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}
