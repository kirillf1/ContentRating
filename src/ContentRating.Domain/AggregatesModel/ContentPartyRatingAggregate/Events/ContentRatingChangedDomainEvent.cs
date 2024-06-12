namespace ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate.Events
{
    public record ContentRatingChangedDomainEvent(Guid ContentRatingId, Guid RoomId, Guid ContentId, ContentRater Rater, Score NewRaterScore, Score AverageContentScore) : INotification;

}
