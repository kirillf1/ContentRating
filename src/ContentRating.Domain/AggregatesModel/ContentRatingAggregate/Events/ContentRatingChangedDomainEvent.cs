using MediatR;

namespace ContentRating.Domain.AggregatesModel.ContentRatingAggregate.Events
{
    public record ContentRatingChangedDomainEvent(Guid RoomId, Guid ContentId, Rater Rater, Score AverageScore) : INotification;

}
