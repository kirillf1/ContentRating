namespace ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.Events
{
    public record AllContentEstimatedDomainEvent(Guid RoomId, IReadOnlyCollection<Rater> Raters) : INotification;
}
