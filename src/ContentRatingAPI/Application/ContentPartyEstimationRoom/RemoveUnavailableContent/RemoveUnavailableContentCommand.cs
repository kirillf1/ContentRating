// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.RemoveUnavailableContent
{
    public record RemoveUnavailableContentCommand(Guid RoomId, Guid ContentId, Guid RemoveContentInitiatorId) : IRequest<Result<bool>>;
}
