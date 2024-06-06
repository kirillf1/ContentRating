namespace ContentRating.Domain.AggregatesModel.ContentPartyRatingRoomAggregate.Events
{
    public record RaterKickedDomainEvent(Rater KickedUser, Guid RoomId) : INotification;
}
