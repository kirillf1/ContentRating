using Bogus;
using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;
using ContentRating.Domain.Shared.Content;

namespace ContentRating.IntegrationTests.DataHelpers
{
    internal static class ContentRoomEditorGenerator
    {
        private static readonly Faker _faker = new Faker();
      
        public static List<ContentRoomEditor> GenerateContentRoomEditors(int count, Guid? editorId = null)
        {
            var contentRoomEditors = new List<ContentRoomEditor>();

            for (int i = 0; i < count; i++)
            {
                ContentRoomEditor roomEditor = GenerateContentRoomEditor(editorId);

                contentRoomEditors.Add(roomEditor);
            }

            return contentRoomEditors;
        }

        public static ContentRoomEditor GenerateContentRoomEditor(Guid? editorId = null)
        {
            var id = Guid.NewGuid();
            var creator = GenerateEditor(editorId);
            var name = _faker.Lorem.Sentence(3, 3);
            var roomEditor = ContentRoomEditor.Create(id, creator, name);

            var contentList = GenerateContentList(creator);
            foreach (var content in contentList)
            {
                roomEditor.CreateContent(creator, content);
            }

            var editors = GenerateEditors();
            foreach (var editor in editors)
            {
                roomEditor.InviteEditor(creator, editor);
            }

            return roomEditor;
        }

        private static Editor GenerateEditor(Guid? editorId = null)
        {
            var id = editorId ?? Guid.NewGuid();
            return new Editor(id, _faker.Name.FullName());
        }

        private static List<Editor> GenerateEditors(int count = 3)
        {
            var editors = new List<Editor>();
            for (int i = 0; i < count; i++)
            {
                editors.Add(GenerateEditor());
            }

            return editors;
        }

        private  static List<ContentData> GenerateContentList(Editor editor, int count = 5)
        {
            var random = new Random();
            var contentTypes = Enum.GetValues(typeof(ContentType));
            var contentList = new List<ContentData>();
            for (int i = 0; i < count; i++)
            {
                var contentType = (ContentType)contentTypes.GetValue(random.Next(0, contentTypes.Length));
                var contentData = new ContentData
                (
                    Guid.NewGuid(),
                    _faker.Internet.Url(),
                    _faker.Lorem.Sentence(3, 3),
                    contentType
                );

                contentList.Add(contentData);
            }

            return contentList;
        }
    }
}
