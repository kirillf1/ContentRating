using MediatR;

namespace ContentRating.Domain.AggregatesModel.RatingContentAggregate.Events
{
    public record ContentRatingChangedDomainEvent(Guid RoomId, Guid ContentId, Rater Rater, Score AverageContentScore) : INotification;
  
}
