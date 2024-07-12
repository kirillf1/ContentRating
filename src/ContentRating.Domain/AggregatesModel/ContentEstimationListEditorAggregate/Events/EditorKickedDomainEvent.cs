namespace ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events
{
    public record EditorKickedDomainEvent(Guid RoomId, ContentEditor Initiator, ContentEditor KickedEditor) : INotification;
}
