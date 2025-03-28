// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRatingAPI.Application.ContentEstimationListEditor.GetContentEstimationListEditor
{
    public class ContentEstimationListEditorResponse
    {
        public ContentEstimationListEditorResponse(
            Guid id,
            string name,
            string creatorName,
            Guid creatorId,
            IEnumerable<ContentResponse> content,
            IEnumerable<InvitedEditorResponse> invitedEditors
        )
        {
            Id = id;
            Name = name;
            CreatorName = creatorName;
            CreatorId = creatorId;
            Content = content;
            InvitedEditors = invitedEditors;
        }

        public Guid Id { get; }
        public string Name { get; }
        public string CreatorName { get; }
        public Guid CreatorId { get; }
        public IEnumerable<ContentResponse> Content { get; }
        public IEnumerable<InvitedEditorResponse> InvitedEditors { get; }
    }
}
