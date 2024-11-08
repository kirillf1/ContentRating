// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events;
using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Exceptions;
using ContentRating.Domain.Shared.RoomStates;

namespace ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate
{
    public class ContentEstimationListEditor : Entity, IAggregateRoot
    {
        public IReadOnlyCollection<Content> AddedContent
        {
            get { return _addedContent; }
            private set { _addedContent = value.ToList(); }
        }
        public IReadOnlyCollection<ContentEditor> InvitedEditors
        {
            get { return _invitedEditors; }
            private set { _invitedEditors = value.ToList(); }
        }
        public ContentEditor ContentListCreator { get; private set; }
        public string Name { get; private set; }

        private List<Content> _addedContent;
        private List<ContentEditor> _invitedEditors;

        public void CreateContent(ContentEditor editor, ContentData contentData)
        {
            if (!_invitedEditors.Contains(editor) && editor != ContentListCreator)
            {
                throw new ForbiddenRoomOperationException("Editor don't exist in this room");
            }

            var newContent = editor.CreateContent(contentData);
            if (_addedContent.Contains(newContent))
            {
                throw new ContentAlreadyAddedException("Same content already added", newContent);
            }
            _addedContent.Add(newContent);

            AddDomainEvent(new ContentAddedToListDomainEvent(newContent, Id));
        }

        public void UpdateContent(ContentEditor editor, ContentData contentModification)
        {
            if (!_invitedEditors.Contains(editor) && editor != ContentListCreator)
            {
                throw new ForbiddenRoomOperationException("Editor don't exist in this room");
            }

            var oldContent = _addedContent.Single(c => c.Id == contentModification.Id);
            var contentEditor = oldContent.ContentModificationHistory.EditorId;
            if (contentEditor != editor.Id && ContentListCreator != editor)
            {
                throw new ForbiddenRoomOperationException("Can't edit foreign content");
            }

            oldContent.ModifyContent(contentModification);

            AddDomainEvent(new ContentUpdatedDomainEvent(oldContent, Id));
        }

        public bool RemoveContent(ContentEditor editor, Content content)
        {
            if (!_invitedEditors.Contains(editor) && editor != ContentListCreator)
            {
                throw new ForbiddenRoomOperationException("Editor don't exist in this room");
            }
            if (content.ContentModificationHistory.EditorId != editor.Id && ContentListCreator != editor)
            {
                throw new ForbiddenRoomOperationException("Can't edit foreign content");
            }

            var isRemoved = _addedContent.Remove(content);
            if (isRemoved)
            {
                AddDomainEvent(new ContentRemovedFromListDomainEvent(content, Id));
            }
            return isRemoved;
        }

        public void InviteEditor(ContentEditor inviter, ContentEditor newEditor)
        {
            if (inviter != ContentListCreator)
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

        public void KickEditor(ContentEditor initiator, ContentEditor editorForKick)
        {
            if (initiator != ContentListCreator)
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
            if (roomName.Length < 3 || roomName.Length > 300)
            {
                throw new ArgumentException("Room name must be more than 3 and less than 300 symbols");
            }
            Name = roomName;
        }

        public void MarkDeleted()
        {
            AddDomainEvent(new RoomDeletedDomainEvent(Id));
        }

        internal ContentEstimationListEditor(Guid id, ContentEditor roomCreator, string name)
        {
            Id = id;
            ContentListCreator = roomCreator;
            Name = name;
            _addedContent = new();
            _invitedEditors = new();
            AddDomainEvent(new ContentRoomEditorCreatedDomainEvent(id, roomCreator, name));
        }

        public static ContentEstimationListEditor Create(Guid id, ContentEditor roomCreator, string name)
        {
            return new ContentEstimationListEditor(id, roomCreator, name);
        }
    }
}
