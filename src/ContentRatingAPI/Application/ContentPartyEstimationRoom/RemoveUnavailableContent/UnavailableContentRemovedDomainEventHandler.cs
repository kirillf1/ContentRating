using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.Events;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.RemoveUnavailableContent
{
    public class UnavailableContentRemovedDomainEventHandler : INotificationHandler<UnavailableContentRemovedDomainEvent>
    {
        private readonly IContentPartyRatingRepository contentPartyRatingRepository;

        public UnavailableContentRemovedDomainEventHandler(IContentPartyRatingRepository contentPartyRatingRepository)
        {
            this.contentPartyRatingRepository = contentPartyRatingRepository;
        }
        public async Task Handle(UnavailableContentRemovedDomainEvent notification, CancellationToken cancellationToken)
        {
            var contentRating = await contentPartyRatingRepository.GetContentRating(notification.RoomId, notification.RemovedContent.Id);

            contentPartyRatingRepository.Delete(contentRating);

            await contentPartyRatingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
