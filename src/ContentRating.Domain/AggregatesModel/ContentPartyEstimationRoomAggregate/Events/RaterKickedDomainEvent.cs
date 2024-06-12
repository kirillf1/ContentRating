namespace ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.Events
{
    public record RaterKickedDomainEvent(Rater KickedRater, Guid RoomId) : INotification;
}
