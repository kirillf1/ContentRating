namespace ContentRating.Domain.AggregatesModel.ContentRoomAggregate.Exceptions
{
    public class UserAlreadyInvitedException(string message, User invitedUser) : Exception(message)
    {
        public User InvitedUser { get; } = invitedUser;
    }
}
