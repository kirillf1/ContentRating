using MediatR;

namespace ContentRating.Domain.AggregatesModel.RoomAccessControlAggregate.Events
{
    public record UserInvitedDomainEvent(User User, Guid RoomId) : INotification;
}
