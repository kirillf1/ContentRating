using MediatR;

namespace ContentRating.Domain.AggregatesModel.RoomEditorAggregate.Events
{
    public record ContentUpdatedInRoomDomainEvent(Content UpdatedContent, Guid RoomId) : INotification;
}
