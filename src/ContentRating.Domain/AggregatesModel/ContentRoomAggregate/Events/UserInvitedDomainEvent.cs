using MediatR;

namespace ContentRating.Domain.AggregatesModel.ContentRoomAggregate.Events
{
    public record UserInvitedDomainEvent(User User, Guid RoomId) : INotification;
}
