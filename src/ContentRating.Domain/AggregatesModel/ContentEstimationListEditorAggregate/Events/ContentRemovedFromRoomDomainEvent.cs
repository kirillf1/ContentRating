using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;

namespace ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events
{
    public record ContentRemovedFromRoomDomainEvent(Content Content, Guid RoomId) : INotification;
}
