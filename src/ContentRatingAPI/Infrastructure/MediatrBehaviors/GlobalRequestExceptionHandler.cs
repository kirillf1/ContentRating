using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate.Exceptions;
using ContentRating.Domain.Shared.RoomStates;
using MediatR.Pipeline;
using System.Reflection;

namespace ContentRatingAPI.Infrastructure.MediatrBehaviors
{
    public class GlobalRequestExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
       where TException : Exception
    {
        private static IEnumerable<Type> InvalidResponseTypes = [typeof(ForbiddenRatingOperationException),
            typeof(ForbiddenRoomOperationException), typeof(InvalidRoomStageOperationException), typeof(ArgumentException)];

        private readonly ILogger<GlobalRequestExceptionHandler<TRequest, TResponse, TException>> _logger;
        public GlobalRequestExceptionHandler(
           ILogger<GlobalRequestExceptionHandler<TRequest, TResponse, TException>> logger)
        {
            _logger = logger;
        }
        public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Something went wrong while handling command of type {CommandName} ({@Command}", typeof(TRequest), request);
            state.SetHandled(CreateErrorResult(typeof(TResponse), exception)!);
            return Task.CompletedTask;
        }

        private static TResponse? CreateErrorResult(Type type, Exception exception)
        {
            object? result = default;
            if (!type.GetInterfaces().Contains(typeof(Ardalis.Result.IResult)))          
                return (TResponse?)result;
            //TODO if need more exception update logic
            var isInvalidException = InvalidResponseTypes.Contains(exception.GetType());
            if (isInvalidException)
            {
                ConstructorInfo? ctor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(ResultStatus) }, null);
                result = ctor?.Invoke([ResultStatus.Invalid]);

                PropertyInfo errorsProperty = type.GetProperty("ValidationErrors")!;
                errorsProperty?.SetValue(result, new ValidationError[] { new ValidationError(exception.Message) });
            }
            else if(exception.GetType() == typeof(ArgumentNullException))
            {
                ConstructorInfo? ctor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(ResultStatus) }, null);
                result = ctor?.Invoke([ResultStatus.NotFound]);
                PropertyInfo errorsProperty = type.GetProperty("Errors")!;
                errorsProperty?.SetValue(result, new string[] { exception.Message });
            }
            else
            {
                ConstructorInfo? ctor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(ResultStatus) }, null);
                result = ctor?.Invoke([ResultStatus.Error]);

                PropertyInfo errorsProperty = type.GetProperty("Errors")!;
                errorsProperty?.SetValue(result, new string[] { exception.Message });
                
            }
            return (TResponse?)result;
        }
    }
}
