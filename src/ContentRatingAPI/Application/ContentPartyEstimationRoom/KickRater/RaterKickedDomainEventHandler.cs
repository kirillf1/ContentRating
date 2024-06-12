using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.Events;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRatingAPI.Application.ContentPartyRating.ContentRaterService;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.KickRater
{
    public class RaterKickedDomainEventHandler : INotificationHandler<RaterKickedDomainEvent>
    {
        private readonly ContentPartyRatingService contentPartyRatingService;

        public RaterKickedDomainEventHandler(ContentPartyRatingService contentPartyRatingService)
        {
            this.contentPartyRatingService = contentPartyRatingService;
        }
        
        public async Task Handle(RaterKickedDomainEvent notification, CancellationToken cancellationToken)
        {
            var kickedRater = notification.KickedRater.MapToContentRater();
            await contentPartyRatingService.RemoveRaterScoreInContentRatingList(notification.RoomId, kickedRater);
        }
    }
}
