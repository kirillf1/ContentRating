// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;
using ContentEstimationListEditorAggregate = ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.ContentEstimationListEditor;

namespace ContentRatingAPI.Application.ContentEstimationListEditor
{
    public static class ContentEstimationListEditorHelper
    {
        public static ContentEditor? TryGetEditorFromRoom(this ContentEstimationListEditorAggregate contentEditorRoom, Guid editorId)
        {
            if (contentEditorRoom.ContentListCreator.Id == editorId)
            {
                return contentEditorRoom.ContentListCreator;
            }

            return contentEditorRoom.InvitedEditors.FirstOrDefault(c => c.Id == editorId);
        }
    }
}
