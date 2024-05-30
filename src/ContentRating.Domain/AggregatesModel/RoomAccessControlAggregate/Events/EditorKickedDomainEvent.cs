namespace ContentRating.Domain.AggregatesModel.RoomAccessControlAggregate.Events
{
    public record EditorKickedDomainEvent(Guid RoomId, User Initiator, User KickedEditor) : INotification;
}
