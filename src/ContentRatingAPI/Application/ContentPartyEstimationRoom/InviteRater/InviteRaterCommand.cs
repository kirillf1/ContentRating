// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.InviteRater
{
    public record InviteRaterCommand(Guid RoomId, Guid InviteInitiatorId, Guid RaterForInviteId, RoleType RoleType, string RaterName)
        : IRequest<Result<bool>>;
}
