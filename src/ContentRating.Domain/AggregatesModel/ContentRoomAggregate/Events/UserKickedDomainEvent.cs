using MediatR;

namespace ContentRating.Domain.AggregatesModel.ContentRoomAggregate.Events
{
    public record UserKickedDomainEvent(User KickedUser, Guid RoomId) : INotification;
}
