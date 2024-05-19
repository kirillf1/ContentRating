using ContentRating.Domain.Shared;
using ContentRating.Domain.Shared.RoomStates;
using ContentRating.Domain.AggregatesModel.RoomAccessControlAggregate.Events;

namespace ContentRating.Domain.AggregatesModel.RoomAccessControlAggregate
{
    public class RoomAccessControl : Entity, IAggregateRoot
    {
        private RoomControlSpecification _roomSpecification;
        public RoomState RoomState { get; private set; }
        public User RoomCreator { get; }
        public IReadOnlyCollection<User> Users => _users.Values;
        private Dictionary<Guid, User> _users;
        public void KickUser(Guid targetUserId, Guid initiatorId)
        {
            if (!_roomSpecification.RoomIsWorking(this))
                throw new InvalidRoomStageOperationException("Сan't kick user when the room is not working");

            if (!_users.TryGetValue(initiatorId, out var initiator) || !_users.TryGetValue(targetUserId, out var userForKick))
            {
                throw new ArgumentException("Room don't contain this user");
            }
            if (!_roomSpecification.CanKickAnotherUser(initiator))
                throw new ForbiddenRoomOperationException("This user does not have the right to kick other users ");

            if (initiator == userForKick)
                throw new ForbiddenRoomOperationException("The user cannot kick himself");

            if (userForKick == RoomCreator)
                throw new ForbiddenRoomOperationException("Cannot kick room creator");

            _users.Remove(targetUserId);

            AddDomainEvent(new UserKickedDomainEvent(userForKick, Id));

        }
        public void InviteUser(User newUser, Guid inviterId)
        {
            if (!_roomSpecification.RoomIsWorking(this))
                throw new InvalidRoomStageOperationException("Сan't invite user when the room is not working");

            if (_users.TryGetValue(newUser.Id, out var _))
            {
                throw new ArgumentException("This user is already invited");
            }
            if (!_users.TryGetValue(inviterId, out var inviter))
            {
                throw new ArgumentException("Unknown inviter");
            }
            if (!_roomSpecification.CanInviteAnotherUser(inviter))
                throw new ForbiddenRoomOperationException("This user does not have the right to kick other users ");

            _users.Add(newUser.Id, newUser);

            AddDomainEvent(new UserInvitedDomainEvent(newUser, Id));

        }
        public void CompleateEvaluation()
        {
            RoomState = RoomState.EvaluationComplete;
        }
        private RoomAccessControl(Guid id, User creator, RoomControlSpecification specification)
        {
            Id = id;
            RoomCreator = creator;
            RoomState = RoomState.Editing;
            _users = new Dictionary<Guid, User>() { { creator.Id, creator } };
            _roomSpecification = specification;
        }
        public static RoomAccessControl Create(Guid id, User creator)
        {
            return new RoomAccessControl(id, creator, new RoomControlSpecification());
        }
    }
}
