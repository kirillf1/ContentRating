// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bogus;
using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;
using ContentRating.Domain.Shared.Content;

namespace ContentRating.IntegrationTests.DataHelpers
{
    internal static class ContentEstimationListEditorGenerator
    {
        private static readonly Faker _faker = new Faker();

        public static List<ContentEstimationListEditor> ContentEstimationListEditors(
            int count,
            Guid? editorId = null
        )
        {
            var editors = new List<ContentEstimationListEditor>();

            for (int i = 0; i < count; i++)
            {
                ContentEstimationListEditor editor =
                    ContentEstimationListEditor(editorId);

                editors.Add(editor);
            }

            return editors;
        }

        public static ContentEstimationListEditor ContentEstimationListEditor(
            Guid? editorId = null
        )
        {
            var id = Guid.NewGuid();
            var creator = GenerateEditor(editorId);
            var name = _faker.Lorem.Sentence(3, 3);
            var roomEditor =
                Domain.AggregatesModel.ContentEstimationListEditorAggregate.ContentEstimationListEditor.Create(
                    id,
                    creator,
                    name
                );

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

        private static ContentEditor GenerateEditor(Guid? editorId = null)
        {
            var id = editorId ?? Guid.NewGuid();
            return new ContentEditor(id, _faker.Name.FullName());
        }

        private static List<ContentEditor> GenerateEditors(int count = 3)
        {
            var editors = new List<ContentEditor>();
            for (int i = 0; i < count; i++)
            {
                editors.Add(GenerateEditor());
            }

            return editors;
        }

        private static List<ContentData> GenerateContentList(
            ContentEditor editor,
            int count = 5
        )
        {
            var random = new Random();
            var contentTypes = Enum.GetValues(typeof(ContentType));
            var contentList = new List<ContentData>();
            for (int i = 0; i < count; i++)
            {
                var contentType = (ContentType)
                    contentTypes.GetValue(random.Next(0, contentTypes.Length));
                var contentData = new ContentData(
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
