namespace ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.Events
{
    public record ContentEstimationStartedDomainEvent(Guid RoomId, IEnumerable<Rater> Raters, 
        IEnumerable<ContentForEstimation> ContentForEstimation, RatingRange RatingRange) : INotification;
}
