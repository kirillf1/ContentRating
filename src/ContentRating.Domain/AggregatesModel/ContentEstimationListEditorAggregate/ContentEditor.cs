// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate
{
    public class ContentEditor : Entity
    {
        public ContentEditor(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Name { get; }

        public Content CreateContent(ContentData contentData)
        {
            var contentCreator = new ContentModificationHistory(Id);
            return new Content(contentData.Id, contentData.Path, contentData.Name, contentData.Type, contentCreator);
        }
    }
}
