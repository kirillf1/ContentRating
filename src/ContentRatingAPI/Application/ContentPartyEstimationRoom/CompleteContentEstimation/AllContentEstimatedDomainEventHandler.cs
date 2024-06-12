using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.Events;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.CompleteContentEstimation
{
    public class AllContentEstimatedDomainEventHandler : INotificationHandler<AllContentEstimatedDomainEvent>
    {
        private readonly ContentPartyRatingService contentPartyRatingService;

        public AllContentEstimatedDomainEventHandler(ContentPartyRatingService contentPartyRatingService)
        {
            this.contentPartyRatingService = contentPartyRatingService;
        }
        public async Task Handle(AllContentEstimatedDomainEvent notification, CancellationToken cancellationToken)
        {
            await contentPartyRatingService.CompleteContentEstimation(notification.RoomId);
        }
    }
}
