// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.Shared.Content;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.ContentModifications
{
    public record CreateContentCommand(Guid RoomId, Guid EditorId, Guid ContentId, string Name, string Url, ContentType ContentType)
        : IRequest<Result<bool>>;
}
