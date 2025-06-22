namespace ContentRating.Web.UI.Services.Communication
{
    public interface IContentPartyEstimationHubService
    {
        bool IsConnected { get; }

        Task ConnectAsync(Guid roomId);
        Task DisconnectAsync();
        Task EstimateContentAsync(Guid contentRatingId, double newRating);

        // События для получения уведомлений
        event Action<Guid, Guid, double>? RatingChanged; // raterId, ratingId, score
        event Action<Guid, string, double>? RaterInvited; // newRaterId, raterName, baseScore
        event Action<Guid>? RaterKicked; // kickedRaterId
        event Action<Guid>? ContentDeleted; // contentId
        event Action? EstimationCompleted;
        event Action<double, double>? RatingRangeChanged; // minRating, maxRating

        // События для управления соединением
        event Action? ConnectionLost; // Соединение потеряно
        event Action? ConnectionRestored; // Соединение восстановлено, нужна перезагрузка данных
    }
}
