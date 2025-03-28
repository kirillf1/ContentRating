// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;
using Microsoft.AspNetCore.Authorization;

namespace ContentRatingAPI.Infrastructure.Authorization
{
    public class ContentEstimationListEditorUserAccessRequirement : IAuthorizationRequirement
    {
        public async Task<bool> Pass(IContentEstimationListEditorRepository contentEditorRoomRepository, Guid editorId, Guid roomId)
        {
            return await contentEditorRoomRepository.HasEditorInContentEstimationList(roomId, editorId);
        }
    }
}
