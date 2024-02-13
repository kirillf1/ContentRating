using MediatR;

namespace ContentRating.Domain.AggregatesModel.ContentRoomAggregate.Events
{
    public record EvaluationStartedDomainEvent(Guid RoomId, IReadOnlyCollection<User> InvitedUsers) : INotification;
    
}
