
using ContentRating.Domain.AggregatesModel.RoomEditorAggregate.Events;
using ContentRating.Domain.AggregatesModel.RoomEditorAggregate.Exceptions;
using ContentRating.Domain.Shared;
using ContentRating.Domain.Shared.RoomStates;

namespace ContentRating.Domain.AggregatesModel.RoomEditorAggregate
{
    public class RoomEditor : Entity, IAggregateRoot
    {
        public RoomEditor(Guid id, Editor creator, string name)
        {
            Id = id;
            Creator = creator;
            Name = name;
            _addedContent = new();
            RoomState = RoomState.Editing;
            _invitedUsers = new();
        }
        public RoomState RoomState { get; private set; }
        public IReadOnlyCollection<Content> AddedContent => _addedContent;
        public IReadOnlyCollection<Editor> InvitedUsers => _invitedUsers;
        public Editor Creator { get; private set; }
        public string Name { get; private set; }

        private List<Content> _addedContent;
        private List<Editor> _invitedUsers;
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
            if (RoomState == RoomState.EvaluationCompleate)
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
        public void AddContent(Editor initiatingUser, Content newContent)
        {
            if (initiatingUser != Creator)
            {
                throw new ForbiddenRoomOperationException("Only creator can add content");
            }
            if (RoomState == RoomState.EvaluationCompleate)
            {
                throw new InvalidRoomStageOperationException("Сan't add content in compleated room");
            }
            if (_addedContent.Contains(newContent))
            {
                throw new ContentAlreadyAddedException("Same content already added", newContent);
            }
            _addedContent.Add(newContent);

            AddDomainEvent(new ContentAddedToRoomDomainEvent(newContent, Id));
        }
        public void UpdateContent(Editor initiatingUser, Guid contentId, ContentModification contentModification)
        {
            if (initiatingUser != Creator)
            {
                throw new ForbiddenRoomOperationException("Only creator can update content");
            }
            if (RoomState == RoomState.EvaluationCompleate)
            {
                throw new InvalidRoomStageOperationException("Сan't update content in compleated room");
            }

            var oldContent = _addedContent.Single(c => c.Id == contentId);
            oldContent.ModifyContent(contentModification);

            AddDomainEvent(new ContentUpdatedInRoomDomainEvent(oldContent, Id));
        }
        public bool RemoveContent(Editor initiatingUser, Content contentForRemove)
        {
            if (initiatingUser != Creator)
            {
                throw new ForbiddenRoomOperationException("Only creator can remove content");
            }
            if (RoomState == RoomState.EvaluationCompleate)
            {
                throw new InvalidRoomStageOperationException("Сan't remove in compleated room");
            }

            var isRemoved = _addedContent.Remove(contentForRemove);
            if (isRemoved)
            {
                AddDomainEvent(new ContentRemovedFromRoomDomaintEvent(contentForRemove, Id));
            }
            return isRemoved;
        }
        public void SetNewRoomName(string roomName)
        {
            if (roomName.Length < 3 || roomName.Length > 300)
            {
                throw new ArgumentException("Room name must be more than 3 and less than 300 symbols");
            }
            Name = roomName;
        }
        public void StartContentEvaluation(Editor initiatingUser)
        {
            if (initiatingUser != Creator)
            {
                throw new ForbiddenRoomOperationException("Only creator can start content evaluation");
            }
            if (RoomState != RoomState.Editing)
            {
                throw new InvalidRoomStageOperationException("Сan't start evaluation if room is not editing state");
            }
            RoomState = RoomState.ContentEvaluation;

            AddDomainEvent(new EvaluationStartedDomainEvent(Id, InvitedUsers));
        }
        public void CompleateContentEvaluation(Editor initiatingUser)
        {
            if (initiatingUser != Creator)
            {
                throw new ForbiddenRoomOperationException("Only creator can compleate content evaluation");
            }
            if (RoomState != RoomState.ContentEvaluation)
            {
                throw new InvalidRoomStageOperationException("Сan't compleate evaluation if room is not evaluation state");
            }
            RoomState = RoomState.EvaluationCompleate;

            AddDomainEvent(new EvaluationCompleatedDomainEvent(Id, InvitedUsers));
        }
        public void MarkDeleted()
        {
            AddDomainEvent(new RoomDeletedDomainEvent(Id));
        }
    }
}
