using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;

namespace ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events
{
    public record ContentUpdatedInRoomDomainEvent(Content UpdatedContent, Guid RoomId) : INotification;
}
