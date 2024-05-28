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
        public bool IsAccessControlStopped { get; private set; } 
        public void KickUser(Guid targetUserId, Guid initiatorId)
        {
            if (IsAccessControlStopped)
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

            AddDomainEvent(new UserKickedDomainEvent(userForKick, Id));

        }
        public void InviteUser(User newUser, Guid inviterId)
        {
            if (IsAccessControlStopped)
                throw new InvalidRoomStageOperationException("Сan't invite user when the room is not working");

            if (_users.Contains(newUser))
            {
                throw new ArgumentException("This user is already invited");
            }

            var inviter = _users.Find(c => c.Id == inviterId) ?? throw new ArgumentException("Unknown inviter");

            if (!_roomSpecification.CanInviteAnotherUser(inviter))
                throw new ForbiddenRoomOperationException("This user does not have the right to kick other users ");

            _users.Contains(newUser);

            AddDomainEvent(new UserInvitedDomainEvent(newUser, Id));

        }
        public void StopAccessControl()
        {
            IsAccessControlStopped = true;
        }
        private RoomAccessControl(Guid id, User creator, RoomControlSpecification specification, List<User> invitedUsersWithCreator)
        { 
            Id = id;
            if(!invitedUsersWithCreator.Contains(creator))
                invitedUsersWithCreator.Add(creator);
            RoomCreator = creator;
            IsAccessControlStopped = false;
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
