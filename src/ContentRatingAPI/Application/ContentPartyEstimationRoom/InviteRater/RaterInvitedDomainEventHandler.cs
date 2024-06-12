using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.Events;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRatingAPI.Application.ContentPartyRating.ContentRaterService;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.InviteRater
{
    public class RaterInvitedDomainEventHandler : INotificationHandler<RaterInvitedDomainEvent>
    {
        private readonly ContentPartyRatingService contentPartyRatingService;

        public RaterInvitedDomainEventHandler(ContentPartyRatingService contentPartyRatingService)
        {
            this.contentPartyRatingService = contentPartyRatingService;
        }
        public async Task Handle(RaterInvitedDomainEvent notification, CancellationToken cancellationToken)
        {          
            var newContentRater = notification.Rater.MapToContentRater();
            await contentPartyRatingService.AddNewRaterScoreInContentRatingList(notification.RoomId, newContentRater);
        }
    }
}
