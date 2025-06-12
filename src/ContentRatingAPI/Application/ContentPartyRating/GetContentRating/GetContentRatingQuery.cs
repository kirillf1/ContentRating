// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Web.Contracts.ContentPartyRating;

namespace ContentRatingAPI.Application.ContentPartyRating.GetContentRating
{
    public record class GetContentRatingQuery(Guid RatingId) : IRequest<Result<ContentPartyRatingResponse>>;
}
