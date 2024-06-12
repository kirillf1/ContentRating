namespace ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.Events
{
    public record RaterInvitedDomainEvent(Rater Rater, Guid RoomId) : INotification;
}
