namespace ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events
{
    public record RoomDeletedDomainEvent(Guid RoomId) : INotification;
}
