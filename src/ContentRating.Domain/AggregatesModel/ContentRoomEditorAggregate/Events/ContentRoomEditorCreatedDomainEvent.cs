namespace ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events
{
    public record ContentRoomEditorCreatedDomainEvent(Guid RoomId, Editor Creator, string Name): INotification;

}
