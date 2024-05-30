using ContentRating.Domain.Shared;
using ContentRating.Domain.Shared.RoomStates;
using ContentRating.Domain.AggregatesModel.RoomAccessControlAggregate.Events;

namespace ContentRating.Domain.AggregatesModel.RoomAccessControlAggregate
{
    public class RoomAccessControl : Entity, IAggregateRoot
    {
        private RoomControlSpecification _roomSpecification;
        public User RoomCreator { get; }
        public IReadOnlyCollection<User> Users => _users;
        private List<User> _users;
        public RoomState RoomState { get; private set; }
        public void KickUser(Guid targetUserId, Guid initiatorId)
        {
            if (RoomState == RoomState.EvaluationComplete)
                throw new InvalidRoomStageOperationException("Сan't kick user when the room is not working");

            var initiator = _users.Find(c => c.Id == initiatorId);
            var userForKick = _users.Find(c => c.Id == targetUserId);
            if (initiator is null || userForKick is null)
            {
                throw new ArgumentException("Room don't contain this user");
            }
            if (!_roomSpecification.CanKickAnotherUser(initiator))
                throw new ForbiddenRoomOperationException("This user does not have the right to kick other users ");

            if (initiator == userForKick)
                throw new ForbiddenRoomOperationException("The user cannot kick himself");

            if (userForKick == RoomCreator)
                throw new ForbiddenRoomOperationException("Cannot kick room creator");

            _users.Remove(userForKick);

            if (RoomState == RoomState.ContentEvaluation)
                AddDomainEvent(new RaterKickedDomainEvent(userForKick, Id));
            
            else if (RoomState == RoomState.Editing)
                AddDomainEvent(new EditorKickedDomainEvent(Id, initiator, userForKick));

        }
        public void InviteUser(User newUser, Guid inviterId)
        {
            if (RoomState == RoomState.EvaluationComplete)
                throw new InvalidRoomStageOperationException("Сan't invite user when the room is not working");

            if (_users.Contains(newUser))
            {
                throw new ArgumentException("This user is already invited");
            }

            var inviter = _users.Find(c => c.Id == inviterId) ?? throw new ArgumentException("Unknown inviter");

            if (!_roomSpecification.CanInviteAnotherUser(inviter))
                throw new ForbiddenRoomOperationException("This user does not have the right to kick other users ");

            _users.Contains(newUser);

            if (RoomState == RoomState.ContentEvaluation)
                AddDomainEvent(new RaterInvitedDomainEvent(newUser, Id));
            else if (RoomState == RoomState.Editing)
                AddDomainEvent(new EditorInvitedDomainEvent(Id, inviter, newUser));
        }
        public UserAccessInformation RequestAccessInformation(Guid userId)
        {
            var user = _users.Find(c=> c.Id == userId);
            if (user is null)
                return new UserAccessInformation(userId, false, false, false, false);
            return new UserAccessInformation(userId, _roomSpecification.CanEditContent(this, user),
                RoomState == RoomState.Editing, _roomSpecification.CanInviteAnotherUser(user), 
                _roomSpecification.CanKickAnotherUser(user));
        }
        public void StartControlContentEvaluationRoom()
        {
            RoomState = RoomState.ContentEvaluation;
        }
        public void StartControlContentEstimatedRoom()
        {
            RoomState = RoomState.EvaluationComplete;
        }

        private RoomAccessControl(Guid id, User creator, RoomControlSpecification specification, List<User> invitedUsersWithCreator)
        { 
            Id = id;
            if(!invitedUsersWithCreator.Contains(creator))
                invitedUsersWithCreator.Add(creator);
            RoomCreator = creator;
            RoomState = RoomState.Editing;
            _users = invitedUsersWithCreator;
            _roomSpecification = specification;
        }
        public static RoomAccessControl Create(Guid id, User creator, List<User>? otherInvitedUsers = null)
        {
            otherInvitedUsers ??= new List<User>();
            otherInvitedUsers.Add(creator);
            return new RoomAccessControl(id, creator, new RoomControlSpecification(), otherInvitedUsers);
        }
    }
}
