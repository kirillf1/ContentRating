using ContentRating.Domain.AggregatesModel.ContentPartyRatingRoomAggregate.Events;
using ContentRating.Domain.Shared.RoomStates;

namespace ContentRating.Domain.AggregatesModel.ContentPartyRatingRoomAggregate
{
    public class ContentPartyRatingRoom : Entity, IAggregateRoot
    {
        private RoomControlSpecification _roomSpecification;
        public Rater RoomCreator { get; }
        public IReadOnlyCollection<Rater> Users => _users;
        private List<Rater> _users;
        public RoomState RoomState { get; private set; }
        public void KickRater(Guid targetUserId, Guid initiatorId)
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
             AddDomainEvent(new RaterKickedDomainEvent(userForKick, Id));

        }
        public void InviteUser(Rater newRater, Guid inviterId)
        {
            if (RoomState == RoomState.EvaluationComplete)
                throw new InvalidRoomStageOperationException("Сan't invite user when the room is not working");

            if (_users.Contains(newRater))
            {
                throw new ArgumentException("This user is already invited");
            }

            var inviter = _users.Find(c => c.Id == inviterId) ?? throw new ArgumentException("Unknown inviter");

            if (!_roomSpecification.CanInviteAnotherUser(inviter))
                throw new ForbiddenRoomOperationException("This user does not have the right to kick other users ");

            _users.Contains(newRater);

            if (RoomState == RoomState.ContentEvaluation)
                AddDomainEvent(new RaterInvitedDomainEvent(newRater, Id));
        }
        public UserAccessInformation RequestAccessInformation(Guid userId)
        {
            var user = _users.Find(c => c.Id == userId);
            if (user is null)
                return new UserAccessInformation(false, false, false, false);
            var canRate = RoomState == RoomState.ContentEvaluation;
            return new UserAccessInformation(_roomSpecification.CanEditContent(this, user),
                canRate, _roomSpecification.CanInviteAnotherUser(user),
                _roomSpecification.CanKickAnotherUser(user), user);
        }
        public void StartControlContentEvaluationRoom()
        {
            RoomState = RoomState.ContentEvaluation;
        }
        public void StartControlContentEstimatedRoom()
        {
            RoomState = RoomState.EvaluationComplete;
        }

        private ContentPartyRatingRoom(Guid id, Rater creator, RoomControlSpecification specification, List<Rater> invitedUsersWithCreator)
        {
            Id = id;
            if (!invitedUsersWithCreator.Contains(creator))
                invitedUsersWithCreator.Add(creator);
            RoomCreator = creator;
            RoomState = RoomState.Editing;
            _users = invitedUsersWithCreator;
            _roomSpecification = specification;
        }
        public static ContentPartyRatingRoom Create(Guid id, Rater creator, List<Rater>? otherInvitedUsers = null)
        {
            otherInvitedUsers ??= new List<Rater>();
            otherInvitedUsers.Add(creator);
            return new ContentPartyRatingRoom(id, creator, new RoomControlSpecification(), otherInvitedUsers);
        }
    }
}
