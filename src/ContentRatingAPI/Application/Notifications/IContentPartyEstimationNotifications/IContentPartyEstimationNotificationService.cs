namespace ContentRatingAPI.Application.Notifications.IContentPartyEstimationNotifications
{
    public interface IContentPartyEstimationNotificationService
    {
        public Task NotifyRatingChanged(Guid roomId, Guid raterId, Guid ratingId, double score);
        public Task NotifyRaterKicked(Guid roomId, Guid kickedRaterId);
        public Task NotifyRaterInvited(Guid roomId, Guid newRaterId, string raterName, double baseScore);
        public Task NotifyRatingRangeChanged(Guid roomId, double minRating, double maxRating);
        public Task NotifyEstimationCompleted(Guid roomId);
        public Task NotifyContentDeleted(Guid roomId, Guid contentId);
    }

}
