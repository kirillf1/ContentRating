using MediatR;

namespace ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events
{
    public record ContentRemovedFromRoomDomaintEvent(Content Content, Guid RoomId) : INotification;
}
