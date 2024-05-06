using ContentRating.Domain.AggregatesModel.RoomEditorAggregate;

namespace ContentRating.Domain.AggregatesModel.RoomEditorAggregate.Exceptions
{
    public class UserAlreadyInvitedException(string message, Editor invitedUser) : Exception(message)
    {
        public Editor InvitedUser { get; } = invitedUser;
    }
}
