using MediatR.Pipeline;

namespace ContentRatingAPI.Infrastructure.MediatrBehaviors
{
    public class GlobalRequestExceptionHandler<TRequest, TResponse, TException, T> : IRequestExceptionHandler<TRequest, TResponse, TException>
       where TResponse : Result<T>
       where TException : Exception
    {
        private readonly ILogger<GlobalRequestExceptionHandler<TRequest, TResponse, TException, T>> _logger;
        public GlobalRequestExceptionHandler(
           ILogger<GlobalRequestExceptionHandler<TRequest, TResponse, TException, T>> logger)
        {
            _logger = logger;
        }
        public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
            CancellationToken cancellationToken)
        {
            
            _logger.LogError(exception, "Something went wrong while handling request of type {@requestType}", typeof(TRequest));
            state.SetHandled(default);
            return Task.CompletedTask;
        }
        private TResponse? GetExceptionResult(string requestName)
        {
            var resultType = typeof(TResponse);

            return (TResponse?)Activator.CreateInstance(typeof(TResponse));

            return default;
        }
    }
}
