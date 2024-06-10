namespace ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate.Events
{
    public record ContentRatingChangedDomainEvent(Guid RoomId, Guid ContentId, ContentRater Rater, Score NewRaterScore, Score AverageContentScore) : INotification;

}
