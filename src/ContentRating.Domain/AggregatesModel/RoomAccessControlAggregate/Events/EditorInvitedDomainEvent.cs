namespace ContentRating.Domain.AggregatesModel.RoomAccessControlAggregate.Events
{
    public record EditorInvitedDomainEvent(Guid RoomId, User Inviter, User NewEditor) : INotification;
}
