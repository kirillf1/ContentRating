using Microsoft.AspNetCore.SignalR;

namespace ContentRatingAPI.Notifications
{
    public interface IContentRatingNotificationService
    {
        public Task NotififyRatingChanged(Guid roomId, Guid raterId, double score);
    }
    public class TestH: IContentRatingNotificationService
    {
        public TestH(IHubContext<> test)
        {
            Test = test;
        }
        public Task NotififyRatingChanged(Guid roomId,Guid raterId, double score)
        {
            Test.Groups(roomId).Send("RatingChanged",  raterId, score);
        }
        public IHubContext<THub> Test { get; }
    }
}
