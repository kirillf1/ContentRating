// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Ardalis.Result.FluentValidation;
using FluentValidation;

namespace ContentRatingAPI.Infrastructure.MediatrBehaviors;

public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> _logger;
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidatorBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidatorBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var typeName = request.GetGenericTypeName();

        _logger.LogInformation("Validating command {CommandType}", typeName);
        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(_validators.Select(c => c.ValidateAsync(context, cancellationToken)));
        var resultErrors = validationResults.SelectMany(c => c.AsErrors()).ToList();
        var failures = validationResults.SelectMany(r => r.Errors).Where(c => c is not null).ToList();

#nullable disable
        if (resultErrors.Count != 0)
        {
            _logger.LogWarning("Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}", typeName, request, failures);

            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var resultType = typeof(TResponse).GetGenericArguments()[0];
                var invalidMethod = typeof(Result<>).MakeGenericType(resultType).GetMethod(nameof(Result.Invalid), [typeof(List<ValidationError>)]);

                if (invalidMethod is not null)
                {
                    return (TResponse)invalidMethod.Invoke(null, new object[] { resultErrors });
                }
            }
            else if (typeof(TResponse) == typeof(Result))
            {
                return (TResponse)(object)Result.Invalid(resultErrors);
            }
            else
            {
                throw new ValidationException(failures);
            }
        }

#nullable enable
        return await next();
    }
}
