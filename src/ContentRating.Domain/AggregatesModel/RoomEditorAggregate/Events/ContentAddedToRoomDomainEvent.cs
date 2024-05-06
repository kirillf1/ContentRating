using MediatR;

namespace ContentRating.Domain.AggregatesModel.RoomEditorAggregate.Events
{
    public record ContentAddedToRoomDomainEvent(Content NewContent, Guid RoomId) : INotification;

}
