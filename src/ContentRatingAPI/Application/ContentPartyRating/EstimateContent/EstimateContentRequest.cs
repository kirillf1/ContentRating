// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRatingAPI.Application.ContentPartyRating.EstimateContent
{
    public class EstimateContentRequest
    {
        public required double NewScore { get; set; }
        public required Guid RaterForChangeScoreId { get; set; }
    }
}
