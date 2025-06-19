// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.StartContentPartyEstimation
{
    public record StartContentPartyEstimationCommand(
        Guid RoomId,
        Guid ContentListId,
        string Name,
        double MinRating,
        double MaxRating,
        Guid CreatorId,
        string CreatorName
    ) : IRequest<Result<bool>>;
}
