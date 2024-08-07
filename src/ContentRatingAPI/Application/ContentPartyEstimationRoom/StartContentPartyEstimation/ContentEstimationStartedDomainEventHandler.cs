using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.Events;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRatingAPI.Application.ContentPartyRating.ContentRaterService;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.StartContentPartyEstimation
{
    public class ContentEstimationStartedDomainEventHandler : INotificationHandler<ContentEstimationStartedDomainEvent>
    {
        private readonly ContentPartyRatingService contentPartyRatingService;

        public ContentEstimationStartedDomainEventHandler(ContentPartyRatingService contentPartyRatingService)
        {
            this.contentPartyRatingService = contentPartyRatingService;
        }
        public async Task Handle(ContentEstimationStartedDomainEvent notification, CancellationToken cancellationToken)
        {
            var minScore = new Score(notification.RatingRange.MinRating.Value);
            var maxScore = new Score(notification.RatingRange.MaxRating.Value);
            var contentRaters = notification.Raters.Select(c => c.MapToContentRater());
            var contentIds = notification.ContentForEstimation.Select(c => c.Id);

            await contentPartyRatingService.StartContentEstimation(contentIds, notification.RoomId, contentRaters, minScore, maxScore);
        }
    }
}
