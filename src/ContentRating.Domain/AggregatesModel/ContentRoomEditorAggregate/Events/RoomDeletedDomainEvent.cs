namespace ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events
{
    public record RoomDeletedDomainEvent(Guid RoomId) : INotification;
}
