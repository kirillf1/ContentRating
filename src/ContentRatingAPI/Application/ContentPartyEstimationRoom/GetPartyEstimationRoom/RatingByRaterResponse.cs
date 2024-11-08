// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.GetPartyEstimationRoom
{
    public class RatingByRaterResponse
    {
        public RatingByRaterResponse(Guid raterId, double rating)
        {
            RaterId = raterId;
            Rating = rating;
        }

        public Guid RaterId { get; }
        public double Rating { get; }
    }
}
