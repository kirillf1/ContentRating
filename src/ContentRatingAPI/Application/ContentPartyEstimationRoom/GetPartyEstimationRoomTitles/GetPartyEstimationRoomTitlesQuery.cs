// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.GetPartyEstimationRoomTitles
{
    public record class GetPartyEstimationRoomTitlesQuery(bool IncludeEstimated, bool IncludeNotEstimated, Guid RelatedWithRaterId)
        : IRequest<Result<IEnumerable<PartyEstimationTitle>>>;
}
