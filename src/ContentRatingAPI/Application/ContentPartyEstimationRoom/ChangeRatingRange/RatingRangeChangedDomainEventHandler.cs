using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.Events;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.ChangeRatingRange
{
    public class RatingRangeChangedDomainEventHandler : INotificationHandler<RatingRangeChangedDomainEvent>
    {
        private readonly ContentPartyRatingService contentPartyRatingService;

        public RatingRangeChangedDomainEventHandler(ContentPartyRatingService contentPartyRatingService)
        {
            this.contentPartyRatingService = contentPartyRatingService;
        }
        public async Task Handle(RatingRangeChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            var minScore = new Score(notification.NewMinRating.Value);
            var maxScore = new Score(notification.NewMaxRating.Value);

            await contentPartyRatingService.ChangeRatingSpecification(notification.RoomId, minScore, maxScore);
        }
    }
}
