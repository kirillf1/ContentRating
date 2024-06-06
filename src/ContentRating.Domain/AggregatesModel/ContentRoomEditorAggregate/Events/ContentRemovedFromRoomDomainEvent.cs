namespace ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events
{
    public record ContentRemovedFromRoomDomainEvent(Content Content, Guid RoomId) : INotification;
}
