// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRatingAPI.Application.ContentPartyRating.GetContentRating
{
    public class ContentPartyRatingResponse
    {
        public ContentPartyRatingResponse(Guid id, double averageRating, IEnumerable<RatingResponse> ratings)
        {
            Id = id;
            AverageRating = averageRating;
            Ratings = ratings;
        }

        public Guid Id { get; }
        public double AverageRating { get; }
        public IEnumerable<RatingResponse> Ratings { get; }
    }
}
