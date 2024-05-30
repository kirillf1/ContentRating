namespace ContentRating.Domain.AggregatesModel.RoomAccessControlAggregate.Events
{
    public record RaterInvitedDomainEvent(User User, Guid RoomId) : INotification;
}
