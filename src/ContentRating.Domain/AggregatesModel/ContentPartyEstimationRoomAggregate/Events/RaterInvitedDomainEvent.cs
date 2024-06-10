namespace ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.Events
{
    public record RaterInvitedDomainEvent(Rater User, Guid RoomId) : INotification;
}
