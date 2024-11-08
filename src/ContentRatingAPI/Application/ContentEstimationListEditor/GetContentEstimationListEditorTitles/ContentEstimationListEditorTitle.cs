// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRatingAPI.Application.ContentEstimationListEditor.GetContentEstimationListEditorTitles
{
    public class ContentEstimationListEditorTitle
    {
        public ContentEstimationListEditorTitle(Guid id, string name, int addedContentCount, string creatorName)
        {
            Id = id;
            Name = name;
            AddedContentCount = addedContentCount;
            CreatorName = creatorName;
        }

        public Guid Id { get; }
        public string Name { get; }
        public int AddedContentCount { get; }
        public string CreatorName { get; }
    }
}
