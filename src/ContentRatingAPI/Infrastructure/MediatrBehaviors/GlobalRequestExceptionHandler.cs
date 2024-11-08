// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate.Exceptions;
using ContentRating.Domain.Shared.RoomStates;
using MediatR.Pipeline;

namespace ContentRatingAPI.Infrastructure.MediatrBehaviors
{
    public class GlobalRequestExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
        where TException : Exception
    {
        private static IEnumerable<Type> InvalidResponseTypes =
        [
            typeof(ForbiddenRatingOperationException),
            typeof(ForbiddenRoomOperationException),
            typeof(InvalidRoomStageOperationException),
            typeof(ArgumentException),
        ];

        private readonly ILogger<GlobalRequestExceptionHandler<TRequest, TResponse, TException>> _logger;

        public GlobalRequestExceptionHandler(ILogger<GlobalRequestExceptionHandler<TRequest, TResponse, TException>> logger)
        {
            _logger = logger;
        }

        public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Something went wrong while handling command of type {CommandName} ({@Command}", typeof(TRequest), request);
            state.SetHandled(CreateErrorResult(typeof(TResponse), exception)!);
            return Task.CompletedTask;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Major Code Smell",
            "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields",
            Justification = "<Ожидание>"
        )]
        private static TResponse? CreateErrorResult(Type type, Exception exception)
        {
            object? result = default;
            if (!type.GetInterfaces().Contains(typeof(Ardalis.Result.IResult)))
            {
                return (TResponse?)result;
            }

            var isInvalidException = InvalidResponseTypes.Contains(exception.GetType());
            if (isInvalidException)
            {
                var ctor = type.GetConstructor(
                    bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new[] { typeof(ResultStatus) },
                    null
                );
                result = ctor?.Invoke([ResultStatus.Invalid]);

                var errorsProperty = type.GetProperty("ValidationErrors")!;
                errorsProperty?.SetValue(result, new ValidationError[] { new ValidationError(exception.Message) });
            }
            else if (exception.GetType() == typeof(ArgumentNullException))
            {
                var ctor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(ResultStatus) }, null);
                result = ctor?.Invoke([ResultStatus.NotFound]);
                var errorsProperty = type.GetProperty("Errors")!;
                errorsProperty?.SetValue(result, new string[] { exception.Message });
            }
            else
            {
                var ctor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(ResultStatus) }, null);
                result = ctor?.Invoke([ResultStatus.Error]);

                var errorsProperty = type.GetProperty("Errors")!;
                errorsProperty?.SetValue(result, new string[] { exception.Message });
            }
            return (TResponse?)result;
        }
    }
}
