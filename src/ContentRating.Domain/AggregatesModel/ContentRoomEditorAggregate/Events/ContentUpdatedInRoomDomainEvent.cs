namespace ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events
{
    public record ContentUpdatedInRoomDomainEvent(Content UpdatedContent, Guid RoomId) : INotification;
}
