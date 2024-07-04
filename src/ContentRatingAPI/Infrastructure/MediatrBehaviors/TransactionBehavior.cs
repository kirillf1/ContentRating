using ContentRatingAPI.Infrastructure.Data;

namespace ContentRatingAPI.Infrastructure.MediatrBehaviors
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly MongoContext mongoContext;
        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> logger;

        public TransactionBehavior(MongoContext mongoContext, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
        {
            this.mongoContext = mongoContext;
            this.logger = logger;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var typeName = request.GetGenericTypeName();
            try
            {
                if (mongoContext.HasActiveTransaction())
                    return await next();

                var transaction = await mongoContext.BeginTransactionAsync(cancellationToken);
                logger.LogInformation("Begin transaction {TransactionId} for {CommandName} ({@Command})", transaction.TransactionId, typeName, request);

                TResponse response = await next();

                logger.LogInformation("Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);
                await mongoContext.CommitAsync(transaction);

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Handling transaction for {CommandName} ({@Command})", typeName, request);

                throw;
            }
        }
    }
}
