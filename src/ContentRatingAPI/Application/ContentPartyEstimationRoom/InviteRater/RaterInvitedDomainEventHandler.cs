using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.Events;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRatingAPI.Application.ContentPartyRating.ContentRaterService;
using ContentRatingAPI.Application.Notifications.IContentPartyEstimationNotifications;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.InviteRater
{
    public class RaterInvitedDomainEventHandler : INotificationHandler<RaterInvitedDomainEvent>
    {
        private readonly ContentPartyRatingService contentPartyRatingService;
        private readonly IContentPartyEstimationNotificationService notificationService;

        public RaterInvitedDomainEventHandler(ContentPartyRatingService contentPartyRatingService, IContentPartyEstimationNotificationService notificationService)
        {
            this.contentPartyRatingService = contentPartyRatingService;
            this.notificationService = notificationService;
        }
        public async Task Handle(RaterInvitedDomainEvent notification, CancellationToken cancellationToken)
        {          
            var newContentRater = notification.Rater.MapToContentRater();
            await contentPartyRatingService.AddNewRaterScoreInContentRatingList(notification.RoomId, newContentRater);

            await notificationService.NotifyRaterInvited(notification.RoomId, newContentRater.RaterId, notification.Rater.Name, notification.BaseRating.Value);
        }
    }
}
