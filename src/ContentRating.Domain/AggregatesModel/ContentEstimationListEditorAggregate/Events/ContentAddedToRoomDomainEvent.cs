using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;

namespace ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events
{
    public record ContentAddedToRoomDomainEvent(Content NewContent, Guid RoomId) : INotification;

}
