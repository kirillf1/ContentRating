namespace ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events
{
    public record ContentRoomEditorCreatedDomainEvent(Guid RoomId, ContentEditor Creator, string Name) : INotification;

}
