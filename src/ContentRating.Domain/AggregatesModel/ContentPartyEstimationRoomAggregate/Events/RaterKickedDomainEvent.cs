namespace ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.Events
{
    public record RaterKickedDomainEvent(Rater KickedUser, Guid RoomId) : INotification;
}
