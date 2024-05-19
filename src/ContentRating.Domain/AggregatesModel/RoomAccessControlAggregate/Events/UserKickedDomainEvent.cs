using MediatR;

namespace ContentRating.Domain.AggregatesModel.RoomAccessControlAggregate.Events
{
    public record UserKickedDomainEvent(User KickedUser, Guid RoomId) : INotification;
}
