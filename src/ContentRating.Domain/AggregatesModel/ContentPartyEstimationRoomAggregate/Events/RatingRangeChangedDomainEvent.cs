namespace ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.Events
{
    public record RatingRangeChangedDomainEvent(Guid RoomId, Rating NewMaxRating, Rating NewMinRating) : INotification;
}
