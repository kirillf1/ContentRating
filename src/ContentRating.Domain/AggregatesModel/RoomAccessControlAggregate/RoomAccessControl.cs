using ContentRating.Domain.AggregatesModel.RoomEditorAggregate.Events;
using ContentRating.Domain.AggregatesModel.RoomEditorAggregate.Exceptions;
using ContentRating.Domain.AggregatesModel.RoomEditorAggregate;
using ContentRating.Domain.Shared;
using ContentRating.Domain.Shared.RoomStates;

namespace ContentRating.Domain.AggregatesModel.RoomAccessControlAggregate
{
    public class RoomAccessControl : Entity, IAggregateRoot
    {
        private RoomAccessControl(Guid id, User creator)
        {
            Id = id;
            RoomCreator = creator;
            RoomState = RoomState.Editing;
        }
        public RoomState RoomState { get; private set; }
        public User RoomCreator { get; }
        public IReadOnlyCollection<User> Users => _users;
        private HashSet<User> _users;
        public User? KickUser(Guid targetUserId, Guid initiatorId)
        {

        }
        public void InviteUser(Editor initiatingUser, Editor invitedUser)
        {
            if (initiatingUser != Creator)
            {
                throw new ForbiddenRoomOperationException("Only creator can invite user");
            }
            if (Creator.Equals(invitedUser) || _invitedUsers.Contains(invitedUser))
            {
                throw new UserAlreadyInvitedException("User has already invited", invitedUser);
            }
            if (RoomState != RoomState.ContentEvaluation)
            {
                throw new InvalidRoomStageOperationException("Сan't invite a user when the room state is not evaluation");
            }
            _invitedUsers.Add(invitedUser);

            AddDomainEvent(new UserInvitedDomainEvent(invitedUser, Id));
        }
        public bool KickUser(Editor initiatingUser, Editor userForKick)
        {
            if (initiatingUser != Creator)
            {
                throw new ForbiddenRoomOperationException("Only creator can kick user");
            }
            if (RoomState == RoomState.EvaluationComplete)
            {
                throw new InvalidRoomStageOperationException("Сan't kick a user when the room is compleated");
            }

            var isRemoved = _invitedUsers.Remove(userForKick);
            if (isRemoved)
            {
                AddDomainEvent(new UserKickedDomainEvent(userForKick, Id));
            }
            return isRemoved;

        }
    }
}
