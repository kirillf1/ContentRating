// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.Shared.Content;

namespace ContentRating.Web.Contracts.ContentPartyEstimationRoom
{
    public class ContentRatingResponse
    {
        public ContentRatingResponse(
            Guid ratingId,
            Guid contentId,
            string name,
            string address,
            ContentType contentType,
            Guid creatorId,
            IEnumerable<RatingByRaterResponse> ratings,
            double averageRating
        )
        {
            RatingId = ratingId;
            ContentId = contentId;
            Name = name;
            Address = address;
            ContentType = contentType;
            CreatorId = creatorId;
            Ratings = ratings;
            AverageRating = averageRating;
        }

        public Guid RatingId { get; }
        public Guid ContentId { get; }
        public string Name { get; }
        public string Address { get; }
        public ContentType ContentType { get; }
        public Guid CreatorId { get; }
        public IEnumerable<RatingByRaterResponse> Ratings { get; }
        public double AverageRating { get; }
    }
} 