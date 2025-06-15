using Microsoft.AspNetCore.SignalR.Client;

namespace ContentRating.Web.UI.Services
{
    public class InfiniteRetryPolicy : IRetryPolicy
    {
        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            // Бесконечные попытки реконнекта с увеличивающимися интервалами
            return retryContext.PreviousRetryCount switch
            {
                0 => TimeSpan.Zero,
                1 => TimeSpan.FromSeconds(2),
                2 => TimeSpan.FromSeconds(5),
                3 => TimeSpan.FromSeconds(10),
                4 => TimeSpan.FromSeconds(15),
                5 => TimeSpan.FromSeconds(30),
                _ => TimeSpan.FromMinutes(1) // Максимум 1 минута между попытками
            };
        }
    }
} 