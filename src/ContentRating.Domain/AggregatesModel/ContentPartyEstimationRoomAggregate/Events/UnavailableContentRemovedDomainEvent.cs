namespace ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.Events
{
    public record UnavailableContentRemovedDomainEvent(Guid RoomId, ContentForEstimation RemovedContent, Rater Initiator) : INotification;
}
