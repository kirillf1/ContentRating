namespace ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.Events
{
    public record UnavailableContentRemovedDomainEvent(Guid RoomId, ContentForEstimation removedContent, Rater Initiator) : INotification;
}
