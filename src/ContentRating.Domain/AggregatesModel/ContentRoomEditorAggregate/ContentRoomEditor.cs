using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events;
using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Exceptions;
using ContentRating.Domain.Shared;
using ContentRating.Domain.Shared.RoomStates;

namespace ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate
{
    public class ContentRoomEditor : Entity, IAggregateRoot
    {
        public ContentRoomEditor(Guid id, Editor roomCreator, string name)
        {
            Id = id;
            RoomCreator = roomCreator;
            Name = name;
            _addedContent = new();
            RoomState = RoomState.Editing;
            _invitedEditors = new();
        }
        public RoomState RoomState { get; private set; }
        public IReadOnlyCollection<Content> AddedContent => _addedContent;
        public IReadOnlyCollection<Editor> InvitedUsers => _invitedEditors;
        public Editor RoomCreator { get; private set; }
        public string Name { get; private set; }

        private List<Content> _addedContent;
        private List<Editor> _invitedEditors;

        public void AddContent(Editor editor, ContentData contentData)
        {
            if (!_invitedEditors.Contains(editor) || editor != RoomCreator)
            {
                throw new ArgumentException("Editor don't exist in this room");
            }
            if (RoomState == RoomState.EvaluationComplete)
            {
                throw new InvalidRoomStageOperationException("Сan't add content in completed room");
            }
            var newContent = editor.CreateContent(contentData);
            if (_addedContent.Contains(newContent))
            {
                throw new ContentAlreadyAddedException("Same content already added", newContent);
            }
            _addedContent.Add(newContent);

            AddDomainEvent(new ContentAddedToRoomDomainEvent(newContent, Id));
        }
        public void UpdateContent(Editor editor, ContentData contentModification)
        {
            if (!_invitedEditors.Contains(editor) || editor != RoomCreator)
            {
                throw new ArgumentException("Editor don't exist in this room");
            }
            if (RoomState == RoomState.EvaluationComplete)
            {
                throw new InvalidRoomStageOperationException("Сan't update content in completed room");
            }

            var oldContent = _addedContent.Single(c => c.Id == contentModification.Id);
            var contentEditor = oldContent.ContentModificationHistory.EditorId;
            if (contentEditor != editor.Id || RoomCreator != editor)
            {
                throw new ForbiddenRoomOperationException("Can't edit foregin content");
            }

            oldContent.ModifyContent(contentModification);

            AddDomainEvent(new ContentUpdatedInRoomDomainEvent(oldContent, Id));
        }
        public bool RemoveContent(Editor editor, Content content)
        {
            if (!_invitedEditors.Contains(editor) || editor != RoomCreator)
            {
                throw new ArgumentException("Editor don't exist in this room");
            }
            if (content.ContentModificationHistory.EditorId != editor.Id || RoomCreator != editor)
            {
                throw new ForbiddenRoomOperationException("Can't edit foregin content");
            }
            if (RoomState == RoomState.EvaluationComplete)
            {
                throw new InvalidRoomStageOperationException("Сan't remove in completed room");
            }

            var isRemoved = _addedContent.Remove(content);
            if (isRemoved)
            {
                AddDomainEvent(new ContentRemovedFromRoomDomaintEvent(content, Id));
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
        public void CompleateEditing(Editor initiatingUser)
        {
            if (initiatingUser != RoomCreator)
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
        public void CompleteContentEvaluation(Editor initiatingUser)
        {
            if (initiatingUser != RoomCreator)
            {
                throw new ForbiddenRoomOperationException("Only creator can complete content evaluation");
            }
            if (RoomState != RoomState.ContentEvaluation)
            {
                throw new InvalidRoomStageOperationException("Сan't complete evaluation if room is not evaluation state");
            }
            RoomState = RoomState.EvaluationComplete;

            AddDomainEvent(new EvaluationCompleatedDomainEvent(Id, InvitedUsers));
        }
        public void MarkDeleted()
        {
            AddDomainEvent(new RoomDeletedDomainEvent(Id));
        }
    }
}
