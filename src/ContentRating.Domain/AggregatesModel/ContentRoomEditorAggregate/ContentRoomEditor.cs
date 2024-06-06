using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events;
using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Exceptions;
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
            _invitedEditors = new();
            MaxRating = Rating.DefaultMaxRating;
            MinRating = Rating.DefaultMinRating;
            AddDomainEvent(new ContentRoomEditorCreatedDomainEvent(id, roomCreator, name));
        }
        public bool IsContentListCreated { get; private set; }
        public IReadOnlyCollection<Content> AddedContent => _addedContent;
        public IReadOnlyCollection<Editor> InvitedEditors => _invitedEditors;
        public Editor RoomCreator { get; private set; }
        public string Name { get; private set; }
        public Rating MaxRating { get; private set; }
        public Rating MinRating { get; private set; }
        private List<Content> _addedContent;
        private List<Editor> _invitedEditors;

        public void CreateContent(Editor editor, ContentData contentData)
        {
            if (!_invitedEditors.Contains(editor) && editor != RoomCreator)
            {
                throw new ForbiddenRoomOperationException("Editor don't exist in this room");
            }
            if (IsContentListCreated)
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
            if (!_invitedEditors.Contains(editor) && editor != RoomCreator)
            {
                throw new ForbiddenRoomOperationException("Editor don't exist in this room");
            }
            if (IsContentListCreated)
            {
                throw new InvalidRoomStageOperationException("Сan't update content in completed room");
            }

            var oldContent = _addedContent.Single(c => c.Id == contentModification.Id);
            var contentEditor = oldContent.ContentModificationHistory.EditorId;
            if (contentEditor != editor.Id && RoomCreator != editor)
            {
                throw new ForbiddenRoomOperationException("Can't edit foreign content");
            }

            oldContent.ModifyContent(contentModification);

            AddDomainEvent(new ContentUpdatedInRoomDomainEvent(oldContent, Id));
        }
        public bool RemoveContent(Editor editor, Content content)
        {
            if (!_invitedEditors.Contains(editor) && editor != RoomCreator)
            {
                throw new ForbiddenRoomOperationException("Editor don't exist in this room");
            }
            if (content.ContentModificationHistory.EditorId != editor.Id && RoomCreator != editor)
            {
                throw new ForbiddenRoomOperationException("Can't edit foreign content");
            }
            if (IsContentListCreated)
            {
                throw new InvalidRoomStageOperationException("Сan't remove in completed room");
            }

            var isRemoved = _addedContent.Remove(content);
            if (isRemoved)
            {
                AddDomainEvent(new ContentRemovedFromRoomDomainEvent(content, Id));
            }
            return isRemoved;
        }
        public void InviteEditor(Editor inviter, Editor newEditor)
        {
            if (IsContentListCreated)
            {
                throw new InvalidRoomStageOperationException("Сan't invite editor for created room");
            }            
            if (inviter != RoomCreator)
            {
                throw new ForbiddenRoomOperationException("Only creator can invite editors");
            }
            if (_invitedEditors.Contains(newEditor))
            {
                throw new ArgumentException("Inviter already added");
            }
            _invitedEditors.Add(newEditor);
            AddDomainEvent(new EditorInvitedDomainEvent(Id, inviter, newEditor));
        }
        public void KickEditor(Editor initiator, Editor editorForKick)
        {
            if (IsContentListCreated)
            {
                throw new InvalidRoomStageOperationException("Сan't invite editor for created room");
            }
            if (initiator != RoomCreator)
            {
                throw new ForbiddenRoomOperationException("Only creator can kick editors");
            }
            if (initiator == editorForKick)
            {
                throw new ForbiddenRoomOperationException("Can't kick itself");
            }
            if (!_invitedEditors.Contains(editorForKick))
            {
                return;
            }
            _invitedEditors.Remove(editorForKick);
            AddDomainEvent(new EditorKickedDomainEvent(Id, initiator, editorForKick));
        }
        public void SetNewRoomName(string roomName)
        {
            if (IsContentListCreated)
            {
                throw new InvalidRoomStageOperationException("Сan't set new name in created room");
            }
            if (roomName.Length < 3 || roomName.Length > 300)
            {
                throw new ArgumentException("Room name must be more than 3 and less than 300 symbols");
            }
            Name = roomName;
        }

        public void CompleteContentEditing(Editor initiatingEditor)
        {
            if (initiatingEditor != RoomCreator)
            {
                throw new ForbiddenRoomOperationException("Only creator can start content evaluation");
            }
            if (IsContentListCreated)
            {
                throw new InvalidRoomStageOperationException("Сan't start evaluation if room is not editing state");
            }
            IsContentListCreated = true;

            AddDomainEvent(new ContentListCreatedDomainEvent(Id, RoomCreator, InvitedEditors, AddedContent, MinRating, MaxRating, Name));
        }
        public void ChangeRatingRange(Editor ratingChanger, Rating minRating, Rating maxRating)
        {
            if (IsContentListCreated)
                throw new ForbiddenRoomOperationException("Rating range can be change only in editing state");

            if (ratingChanger != RoomCreator)
                throw new ForbiddenRoomOperationException("Only creator can change rating range");

            if (minRating.Value > maxRating.Value)
                throw new ArgumentException("Min rating can't be more than max");

            MinRating = minRating;
            MaxRating = maxRating;
        }
        public void MarkDeleted()
        {
            AddDomainEvent(new RoomDeletedDomainEvent(Id));
        }
    }
}
