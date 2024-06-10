namespace ContentRatingAPI.Notifications
{
    public interface IContentRatingNotificationService
    {
        public Task NotififyRatingChanged(Guid roomId, Guid raterId, double score);
    }
    
}
