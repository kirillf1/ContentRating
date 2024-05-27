using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;
using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events;
using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Exceptions;
using ContentRating.Domain.Shared.RoomStates;
using Xunit;

namespace ContentRating.Domain.Tests.ContentRoomEditorAggregate
{
    public class ContentRoomEditorTests
    {
        static Random random = new Random();
        [Fact]
        public void CreateContent_NewContent_AddContentUpdatedInRoomEvent()
        {
            var room = CreateEmptyRoomEditor();
            var newContentData = CreateRandomContentData();
            var editor = new Editor(Guid.NewGuid(), "new_editor");

            room.InviteEditor(room.RoomCreator, editor);
            room.CreateContent(editor, newContentData);
            var contentAddedEvent = room.DomainEvents.OfType<ContentAddedToRoomDomainEvent>().FirstOrDefault();

            Assert.NotNull(contentAddedEvent);
            Assert.Equal(newContentData.Id, contentAddedEvent.NewContent.Id);
            Assert.Equal(room.Id, contentAddedEvent.RoomId);
        }
        [Fact]
        public void CreateContent_ExistingContent_ThrowContentAlreadyAddedException()
        {
            var room = CreateEmptyRoomEditor();
            var newContentData = CreateRandomContentData();
            var editor = room.RoomCreator;

            room.CreateContent(editor, newContentData);

            Assert.Throws<ContentAlreadyAddedException>(() => room.CreateContent(editor, newContentData));
        }

        [Fact]
        public void CreateContent_KickedEditor_ThrowForbiddenRoomOperationException()
        {
            var room = CreateEmptyRoomEditor();
            var newContentData = CreateRandomContentData();
            var editor = new Editor(Guid.NewGuid(), "unknown");

            room.InviteEditor(room.RoomCreator, editor);
            room.KickEditor(room.RoomCreator, editor);

            Assert.Throws<ForbiddenRoomOperationException>(() => room.CreateContent(editor, newContentData));
        }
        [Fact]
        public void CreateContent_EvaluationCompleted_ThrowInvalidRoomStageOperation()
        {
            var room = CreateEmptyRoomEditor();
            var newContentData = CreateRandomContentData();
            var editor = room.RoomCreator;

            room.CompleteContentEditing(editor);
            room.CompleteContentEvaluation(editor);

            Assert.Throws<InvalidRoomStageOperationException>(() => room.CreateContent(editor, newContentData));
        }
        [Fact]
        public void UpdateContent_SameContentEditor_AddContentUpdatedInRoomEvent()
        {
            var room = CreateEmptyRoomEditor();
            var contentData = CreateRandomContentData();
            var editor = new Editor(Guid.NewGuid(), "new_editor");
            room.InviteEditor(room.RoomCreator, editor);
            room.CreateContent(editor, contentData);

            var newContentData = new ContentData(contentData.Id, "new_name", "/new_path", ContentType.Image);
            room.UpdateContent(editor, newContentData);
            var contentUpdatedEvent = room.DomainEvents.OfType<ContentUpdatedInRoomDomainEvent>().FirstOrDefault();

            Assert.NotNull(contentUpdatedEvent);
            Assert.Equal(newContentData.Name, contentUpdatedEvent.UpdatedContent.Name);
            Assert.Equal(newContentData.Id, contentUpdatedEvent.UpdatedContent.Id);
            Assert.Equal(newContentData.Type, contentUpdatedEvent.UpdatedContent.Type);
            Assert.Equal(newContentData.Path, contentUpdatedEvent.UpdatedContent.Path);

        }
        [Fact]
        public void UpdateContent_CreatedByNewEditorModifiedByCreator_AddContentUpdatedInRoomEvent()
        {
            var room = CreateEmptyRoomEditor();
            var contentData = CreateRandomContentData();
            var editor = new Editor(Guid.NewGuid(), "new_editor");
            room.InviteEditor(room.RoomCreator, editor);
            room.CreateContent(editor, contentData);

            var newContentData = new ContentData(contentData.Id, "new_name", "/new_path", ContentType.Image);
            room.UpdateContent(room.RoomCreator, newContentData);
            var contentUpdatedEvent = room.DomainEvents.OfType<ContentUpdatedInRoomDomainEvent>().FirstOrDefault();

            Assert.NotNull(contentUpdatedEvent);
            Assert.Equal(newContentData.Name, contentUpdatedEvent.UpdatedContent.Name);
            Assert.Equal(newContentData.Id, contentUpdatedEvent.UpdatedContent.Id);
            Assert.Equal(newContentData.Type, contentUpdatedEvent.UpdatedContent.Type);
            Assert.Equal(newContentData.Path, contentUpdatedEvent.UpdatedContent.Path);

        }
        [Fact]
        public void UpdateContent_ContentNotBelongThisEditor_ThrowForbiddenRoomOperationException()
        {
            var room = CreateEmptyRoomEditor();
            var contentData = CreateRandomContentData();
            var foreignEditor = new Editor(Guid.NewGuid(), "new_editor");
            room.InviteEditor(room.RoomCreator, foreignEditor);

            room.CreateContent(room.RoomCreator, contentData);
            var newContentData = new ContentData(contentData.Id, "new_name", "/new_path", ContentType.Image);

            Assert.Throws<ForbiddenRoomOperationException>(()=> room.UpdateContent(foreignEditor, newContentData));
        }
        [Fact]
        public void CompleteEvaluation_WithContent_AddEvaluationCompletedEvent()
        {
            var room = CreateEmptyRoomEditor();
            var contentData = CreateRandomContentData();
            room.CreateContent(room.RoomCreator, contentData);
            var content = room.AddedContent.Last();

            room.CompleteContentEditing(room.RoomCreator);
            room.CompleteContentEvaluation(room.RoomCreator);

            var evaluationStartedEvent = room.DomainEvents.OfType<EvaluationStartedDomainEvent>().FirstOrDefault();
            var evaluationCompletedEvent = room.DomainEvents.OfType<EvaluationCompletedDomainEvent>().FirstOrDefault();
            Assert.NotNull(evaluationStartedEvent);
            Assert.NotNull(evaluationCompletedEvent);
            Assert.Contains(content, evaluationStartedEvent.AddedContent);
            Assert.Equal(room.RoomCreator, evaluationStartedEvent.Creator);
            Assert.Equal(room.Id, evaluationCompletedEvent.RoomId);
        }
        private static ContentRoomEditor CreateEmptyRoomEditor()
        {
            var editor = new Editor(Guid.NewGuid(), "test");
            return new ContentRoomEditor(Guid.NewGuid(), editor, random.Next().ToString());
        }
        private static ContentData CreateRandomContentData()
        {
            return new ContentData(Guid.NewGuid(), random.Next().ToString(), $"/videos/{random.Next()}", ContentType.Video);
        }
    }
}
