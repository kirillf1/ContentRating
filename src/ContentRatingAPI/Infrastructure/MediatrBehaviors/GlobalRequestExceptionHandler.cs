using Ardalis.Result;
using MediatR.Pipeline;
using System.Reflection;

namespace ContentRatingAPI.Infrastructure.MediatrBehaviors
{
    public class GlobalRequestExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
       where TException : Exception
    {
        private readonly ILogger<GlobalRequestExceptionHandler<TRequest, TResponse, TException>> _logger;
        public GlobalRequestExceptionHandler(
           ILogger<GlobalRequestExceptionHandler<TRequest, TResponse, TException>> logger)
        {
            _logger = logger;
        }
        public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Something went wrong while handling request of type {@requestType}", typeof(TRequest));
            state.SetHandled(CreateErrorResult(typeof(TResponse), exception.Message));
            return Task.CompletedTask;
        }
        private TResponse? GetExceptionResult(string requestName)
        {
            var resultType = typeof(TResponse);

            return (TResponse?)Activator.CreateInstance(typeof(TResponse));

            return default;
        }
        public static TResponse? CreateErrorResult(Type type, string errorMessage)
        {
            //Type resultType = type.MakeGenericType(type);
            ConstructorInfo ctor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(ResultStatus) }, null);
            object result = ctor.Invoke(new object[] { ResultStatus.Error });

            PropertyInfo errorsProperty = type.GetProperty("Errors");
            errorsProperty.SetValue(result, new string[] { errorMessage });

            return (TResponse)result;
        }
    }
}
