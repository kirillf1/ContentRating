// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.SignalR;

namespace ContentRatingAPI.Hubs.Filters
{
    public class LoggingHubFilter : IHubFilter
    {
        private readonly ILogger<LoggingHubFilter> logger;

        public LoggingHubFilter(ILogger<LoggingHubFilter> logger)
        {
            this.logger = logger;
        }

        public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
        {
            logger.LogInformation(
                "Calling hub method: {method}, connectionId: {connectionId}",
                invocationContext.HubMethodName,
                invocationContext.Context.ConnectionId
            );
            try
            {
                var result = await next(invocationContext);

                logger.LogInformation(
                    "Calling hub method: {method} completed, connectionId: {connectionId}",
                    invocationContext.HubMethodName,
                    invocationContext.Context.ConnectionId
                );

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    "Exception calling {method}, connectionId: {connectionId}, error: {ex}",
                    invocationContext.HubMethodName,
                    invocationContext.Context.ConnectionId,
                    ex
                );
                throw;
            }
        }

        async Task IHubFilter.OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
        {
            logger.LogInformation("Connection created, connectionId: {connectionId}", context.Context.ConnectionId);
            await next(context);
        }

        async Task IHubFilter.OnDisconnectedAsync(HubLifetimeContext context, Exception? exception, Func<HubLifetimeContext, Exception?, Task> next)
        {
            logger.LogInformation("Connection closed, connectionId: {connectionId}", context.Context.ConnectionId);
            await next(context, exception);
        }
    }
}
