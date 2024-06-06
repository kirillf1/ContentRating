namespace ContentRating.Domain.AggregatesModel.ContentPartyRatingRoomAggregate.Events
{
    public record RaterInvitedDomainEvent(Rater User, Guid RoomId) : INotification;
}
