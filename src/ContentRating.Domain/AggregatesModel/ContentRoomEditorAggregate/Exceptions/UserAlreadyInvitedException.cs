namespace ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Exceptions
{
    public class UserAlreadyInvitedException(string message, Editor invitedUser) : Exception(message)
    {
        public Editor InvitedUser { get; } = invitedUser;
    }
}
