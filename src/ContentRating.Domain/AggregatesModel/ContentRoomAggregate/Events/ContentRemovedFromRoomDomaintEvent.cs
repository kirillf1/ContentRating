using MediatR;

namespace ContentRating.Domain.AggregatesModel.ContentRoomAggregate.Events
{
    public record ContentRemovedFromRoomDomaintEvent(Content Content, Guid RoomId) : INotification;
}
