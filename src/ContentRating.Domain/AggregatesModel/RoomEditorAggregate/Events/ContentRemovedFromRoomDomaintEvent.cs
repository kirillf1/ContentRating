using MediatR;

namespace ContentRating.Domain.AggregatesModel.RoomEditorAggregate.Events
{
    public record ContentRemovedFromRoomDomaintEvent(Content Content, Guid RoomId) : INotification;
}
