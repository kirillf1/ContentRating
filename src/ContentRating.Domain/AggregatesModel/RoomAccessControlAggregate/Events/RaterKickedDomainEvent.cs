namespace ContentRating.Domain.AggregatesModel.RoomAccessControlAggregate.Events
{
    public record RaterKickedDomainEvent(User KickedUser, Guid RoomId) : INotification;
}
