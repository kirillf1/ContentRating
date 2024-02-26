using MediatR;

namespace ContentRating.Domain.AggregatesModel.ContentRoomAggregate.Events
{
    public record RoomDeletedDomainEvent(Guid RoomId) : INotification;
}
